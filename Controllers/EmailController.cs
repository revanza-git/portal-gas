using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Admin.Interfaces.Repositories;
using Admin.Models.User;

namespace Admin.Controllers
{
    public class EmailController : Controller
    {
        public static IConfigurationRoot Configuration { get; set; }
        IEmailRepository repository;

        public EmailController(IEmailRepository repo)
        {
            repository = repo;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
        }

        public async Task Send()
        {
            IList<Email> emails = repository.Emails.ToList<Email>();
            
            foreach (var email in emails)
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(Configuration["Email:FromName"], Configuration["Email:FromEmail"]));
                emailMessage.To.Add(new MailboxAddress("", email.Receiver));
                emailMessage.Subject = email.Subject;
                emailMessage.Body = new TextPart("html") { Text = email.Message };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(Configuration["Email:SmtpServer"], 587, SecureSocketOptions.StartTlsWhenAvailable).ConfigureAwait(false);
                    await client.AuthenticateAsync(Configuration["Email:SmtpUser"], Configuration["Email:SmtpPassword"]);
                    await client.SendAsync(emailMessage).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
                repository.UpdateStatus(email.EmailID);
            }

        }
    }
}
