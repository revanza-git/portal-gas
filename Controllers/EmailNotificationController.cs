using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Admin.Models;
using Admin.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Admin.Models.User;
using Admin.Interfaces.Repositories;
using Admin.Interfaces.Services;
using Admin.Services;

namespace Admin.Controllers
{
    public class EmailNotificationController : Controller
    {
        private readonly IAmanRepository amanrepository;
        private readonly ISemarRepository semarrepository;
        private readonly ICommonRepository crepository;
        private readonly IEmailRepository emailrepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApiHelper apiHelper;
        private readonly IConfiguration configuration;
        private readonly IEmailService emailService;

        public EmailNotificationController(UserManager<ApplicationUser> userManager, IAmanRepository arepository, ISemarRepository srepository, ICommonRepository common, IEmailRepository erepository, ApiHelper apiHelper, IConfiguration configuration, IEmailService emailService)
        {
            this.userManager = userManager;
            this.semarrepository = srepository;
            this.amanrepository = arepository;
            this.emailrepository = erepository;
            this.crepository = common;
            this.apiHelper = apiHelper;
            this.configuration = configuration;
            this.emailService = emailService;
        }

        public async Task Index()
        {
            var semars = semarrepository.Semars.Where(
                x => (x.ExpiredDate.AddDays(-7) < DateTime.Now && x.Status == 2 && x.ExpiredNotification == 0) ||
                (x.ExpiredDate.AddDays(-3) < DateTime.Now && x.Status == 2 && x.ExpiredNotification == 1) ||
                (x.ExpiredDate.AddDays(-1) < DateTime.Now && x.Status == 2 && x.ExpiredNotification == 2) ||
                (x.ExpiredDate < DateTime.Now && x.Status == 2 && x.ExpiredNotification == 3)
            ).ToList();

            foreach (var semar in semars)
            {
                string expiredRemark = semar.ExpiredNotification switch
                {
                    0 => "SEMAR berikut akan expired dalam 7 hari:",
                    1 => "SEMAR berikut akan expired dalam 3 hari:",
                    2 => "SEMAR berikut akan expired dalam 1 hari:",
                    _ => "SEMAR berikut telah expired:"
                };

                var users = userManager.Users.Where(x => x.Department == semar.Owner).ToList();
                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin") || roles.Contains("AtasanAdmin"))
                    {
                        await emailService.SendTemplatedEmailAsync(
                            "SEMAR_EXPIRY",
                            user.Email,
                            new
                            {
                                SemarID = semar.SemarID,
                                RecipientName = user.Name,
                                ExpiryStatus = expiredRemark,
                                Type = crepository.GetSemarTypes().FirstOrDefault(x => x.SemarTypeID == semar.Type)?.Deskripsi,
                                NoDocument = semar.NoDocument,
                                Title = semar.Title,
                                Level = crepository.GetSemarLevels().FirstOrDefault(x => x.SemarLevelID == semar.SemarLevel)?.Deskripsi,
                                Owner = crepository.GetAllDepartments().FirstOrDefault(x => x.DepartmentID == semar.Owner)?.Deskripsi,
                                Description = semar.Description,
                                Revision = semar.Revision,
                                Classification = crepository.GetClassifications().FirstOrDefault(x => x.ClassificationID == semar.Classification)?.Deskripsi,
                                PublishedDate = semar.PublishDate.ToString("dd MMMM yyyy"),
                                ExpiredDate = semar.ExpiredDate.ToString("dd MMMM yyyy")
                            },
                            "id",
                            EmailPriority.High,
                            "SEMAR"
                        );
                    }
                }
                semarrepository.UpdateExpiredNotif(semar.SemarID);
            }

            var amans = amanrepository.Amans.Where(
                x =>
                    (x.EndDate < DateTime.Now && x.Status == 2 && x.OverdueNotif == 0) ||
                    (x.EndDate.AddDays(1) < DateTime.Now && x.Status == 2 && x.OverdueNotif == 1) ||
                    (x.EndDate.AddDays(2) < DateTime.Now && x.Status == 2 && x.OverdueNotif == 2)
            ).ToList();

            foreach (var aman in amans)
            {
                var responsible = await userManager.FindByNameAsync(aman.Responsible);
                var users = userManager.Users.Where(x => x.Department == responsible.Department && x.UserName != aman.Responsible).ToList();
                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles.Contains("AtasanAdmin"))
                    {
                        await emailService.SendTemplatedEmailAsync(
                            "AMAN_OVERDUE",
                            user.Email,
                            new
                            {
                                AmanID = aman.AmanID,
                                RecipientName = user.Name,
                                StartDate = aman.StartDate.ToString("dd MMMM yyyy"),
                                EndDate = aman.EndDate.ToString("dd MMMM yyyy"),
                                Source = crepository.GetAmanSources().FirstOrDefault(x => x.AmanSourceID == aman.Source)?.Deskripsi,
                                Location = crepository.GetLocations().FirstOrDefault(x => x.LocationID == aman.Location)?.Deskripsi,
                                Findings = aman.Findings,
                                Recommendation = aman.Recommendation,
                                ResponsibleName = responsible.Name,
                                ResponsibleEmail = responsible.Email,
                                VerifierName = (await userManager.FindByNameAsync(aman.Verifier)).Name,
                                Status = crepository.GetAmanStatuses().FirstOrDefault(x => x.AmanStatusID == aman.Status)?.Deskripsi
                            },
                            "id",
                            EmailPriority.High,
                            "AMAN"
                        );
                    }
                }

                amanrepository.UpdateOverdueNotif(aman.AmanID);
            }
        }
    }
}
