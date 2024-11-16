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

        public EmailNotificationController(UserManager<ApplicationUser> userManager, IAmanRepository arepository, ISemarRepository srepository, ICommonRepository common, IEmailRepository erepository, ApiHelper apiHelper, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.semarrepository = srepository;
            this.amanrepository = arepository;
            this.emailrepository = erepository;
            this.crepository = common;
            this.apiHelper = apiHelper;
            this.configuration = configuration;
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
                var message = new StringBuilder();
                message.Append("<table>");
                message.Append($"<tr><td>Id.</td><td>:</td><td>{semar.SemarID}</td></tr>");
                message.Append($"<tr><td>Type</td><td>:</td><td>{crepository.GetSemarTypes().FirstOrDefault(x => x.SemarTypeID == semar.Type)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>No. Dokumen</td><td>:</td><td>{semar.NoDocument}</td></tr>");
                message.Append($"<tr><td>Title</td><td>:</td><td>{semar.Title}</td></tr>");
                message.Append($"<tr><td>Level</td><td>:</td><td>{crepository.GetSemarLevels().FirstOrDefault(x => x.SemarLevelID == semar.SemarLevel)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Owner</td><td>:</td><td>{crepository.GetAllDepartments().FirstOrDefault(x => x.DepartmentID == semar.Owner)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Description</td><td>:</td><td>{semar.Description}</td></tr>");
                message.Append($"<tr><td>Revision</td><td>:</td><td>{semar.Revision}</td></tr>");
                message.Append($"<tr><td>Classification</td><td>:</td><td>{crepository.GetClassifications().FirstOrDefault(x => x.ClassificationID == semar.Classification)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Published Date</td><td>:</td><td>{semar.PublishDate:dd MMMM yyyy}</td></tr>");
                message.Append($"<tr><td>Expired Date</td><td>:</td><td>{semar.ExpiredDate:dd MMMM yyyy}</td></tr>");
                message.Append("</table>");

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
                        var email = new Email
                        {
                            Receiver = user.Email,
                            Subject = "Expired SEMAR Notification",
                            Message = $"Dear {user.Name},<br/><p>{expiredRemark}</p>{message}",
                            Schedule = DateTime.Now,
                            CreatedOn = DateTime.Now
                        };
                        emailrepository.Save(email);
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
                var message = new StringBuilder();
                message.Append("<table>");
                message.Append($"<tr><td>No.</td><td>:</td><td>{aman.AmanID}</td></tr>");
                message.Append($"<tr><td>Start Date</td><td>:</td><td>{aman.StartDate:dd MMMM yyyy}</td></tr>");
                message.Append($"<tr><td>End Date</td><td>:</td><td>{aman.EndDate:dd MMMM yyyy}</td></tr>");
                message.Append($"<tr><td>Source</td><td>:</td><td>{crepository.GetAmanSources().FirstOrDefault(x => x.AmanSourceID == aman.Source)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Location</td><td>:</td><td>{crepository.GetLocations().FirstOrDefault(x => x.LocationID == aman.Location)?.Deskripsi}</td></tr>");
                message.Append($"<tr><td>Findings / Opportunities</td><td>:</td><td>{aman.Findings}</td></tr>");
                message.Append($"<tr><td>Recommendation</td><td>:</td><td>{aman.Recommendation}</td></tr>");
                message.Append($"<tr><td>Responsible</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Responsible)).Name}</td></tr>");
                message.Append($"<tr><td>Email</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Responsible)).Email}</td></tr>");
                message.Append($"<tr><td>Verifier</td><td>:</td><td>{(await userManager.FindByNameAsync(aman.Verifier)).Name}</td></tr>");
                message.Append($"<tr><td>Status</td><td>:</td><td>{crepository.GetAmanStatuses().FirstOrDefault(x => x.AmanStatusID == aman.Status)?.Deskripsi}</td></tr>");
                message.Append("</table>");

                var responsible = await userManager.FindByNameAsync(aman.Responsible);
                var users = userManager.Users.Where(x => x.Department == responsible.Department && x.UserName != aman.Responsible).ToList();
                foreach (var user in users)
                {
                    var roles = await userManager.GetRolesAsync(user);
                    if (roles.Contains("AtasanAdmin"))
                    {
                        var email = new Email
                        {
                            Receiver = user.Email,
                            Subject = "Overdue AMAN Notification",
                            Message = $"Dear {user.Name},<br/><p>AMAN berikut telah mengalami keterlambatan:</p>{message}",
                            Schedule = DateTime.Now,
                            CreatedOn = DateTime.Now
                        };
                        emailrepository.Save(email);
                    }
                }

                amanrepository.UpdateOverdueNotif(aman.AmanID);
            }
            await apiHelper.SendEmailAsync();
        }
    }
}
