using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Admin.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private ApplicationDbContext context;
        public EmailRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public void Save(Email email)
        {
            context.Emails.Add(email);
            context.SaveChanges();
        }

        public IEnumerable<Email> Emails => context.Emails.Where(x => x.Status == 0);

        public Email UpdateStatus(int EmailID)
        {
            Email search = context.Emails.FirstOrDefault(x => x.EmailID == EmailID);
            if (search != null)
            {
                search.Status = 1;
                context.SaveChanges();
            }
            return search;
        }

        public Email GetById(int emailId)
        {
            return context.Emails.FirstOrDefault(x => x.EmailID == emailId);
        }

        public IEnumerable<Email> GetPendingEmails()
        {
            return context.Emails.Where(x => x.Status == (int)EmailStatus.Pending || x.Status == (int)EmailStatus.Retry);
        }

        public IEnumerable<Email> GetFailedEmails()
        {
            return context.Emails.Where(x => x.Status == (int)EmailStatus.Failed);
        }

        public IEnumerable<Email> GetEmailsByStatus(EmailStatus status)
        {
            return context.Emails.Where(x => x.Status == (int)status);
        }

        public IEnumerable<Email> GetEmailsByCategory(string category)
        {
            return context.Emails.Where(x => x.Category == category);
        }

        public IEnumerable<Email> GetEmailsByPriority(EmailPriority priority)
        {
            return context.Emails.Where(x => x.Priority == (int)priority);
        }

        public IEnumerable<Email> GetEmailsInDateRange(DateTime from, DateTime to)
        {
            return context.Emails.Where(x => x.CreatedOn >= from && x.CreatedOn <= to);
        }

        public void UpdateEmailForRetry(int emailId, string errorMessage)
        {
            var email = context.Emails.FirstOrDefault(x => x.EmailID == emailId);
            if (email != null)
            {
                email.Status = (int)EmailStatus.Retry;
                email.RetryCount++;
                email.ErrorMessage = errorMessage;
                email.Schedule = DateTime.Now.AddMinutes(GetRetryDelayMinutes(email.RetryCount));
                context.SaveChanges();
            }
        }

        public void UpdateEmailStatus(int emailId, EmailStatus status, string errorMessage = null, DateTime? sentOn = null)
        {
            var email = context.Emails.FirstOrDefault(x => x.EmailID == emailId);
            if (email != null)
            {
                email.Status = (int)status;
                email.ErrorMessage = errorMessage;
                
                if (status == EmailStatus.Sent && sentOn.HasValue)
                {
                    email.SentOn = sentOn.Value;
                }
                else if (status == EmailStatus.Sent)
                {
                    email.SentOn = DateTime.Now;
                }
                
                context.SaveChanges();
            }
        }

        public async Task<int> GetEmailCountByStatusAsync(EmailStatus status)
        {
            return await context.Emails.CountAsync(x => x.Status == (int)status);
        }

        public async Task<Dictionary<string, int>> GetEmailStatsByCategory()
        {
            return await context.Emails
                .GroupBy(x => x.Category ?? "Unknown")
                .ToDictionaryAsync(g => g.Key, g => g.Count());
        }

        public void BulkUpdateStatus(IEnumerable<int> emailIds, EmailStatus status)
        {
            var emails = context.Emails.Where(x => emailIds.Contains(x.EmailID));
            foreach (var email in emails)
            {
                email.Status = (int)status;
                if (status == EmailStatus.Sent)
                {
                    email.SentOn = DateTime.Now;
                }
            }
            context.SaveChanges();
        }

        public void DeleteOldEmails(DateTime olderThan)
        {
            var oldEmails = context.Emails.Where(x => x.CreatedOn < olderThan && 
                (x.Status == (int)EmailStatus.Sent || x.Status == (int)EmailStatus.Failed));
            
            context.Emails.RemoveRange(oldEmails);
            context.SaveChanges();
        }

        private int GetRetryDelayMinutes(int retryCount)
        {
            // Exponential backoff: 2, 4, 8, 16, 32 minutes
            return (int)Math.Pow(2, retryCount);
        }
    }
}
