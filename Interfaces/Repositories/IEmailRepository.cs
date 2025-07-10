using Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IEmailRepository
    {
        void Save(Email email);
        IEnumerable<Email> Emails { get; }
        Email UpdateStatus(int EmailID);
        Email GetById(int emailId);
        IEnumerable<Email> GetPendingEmails();
        IEnumerable<Email> GetFailedEmails();
        IEnumerable<Email> GetEmailsByStatus(EmailStatus status);
        IEnumerable<Email> GetEmailsByCategory(string category);
        IEnumerable<Email> GetEmailsByPriority(EmailPriority priority);
        IEnumerable<Email> GetEmailsInDateRange(DateTime from, DateTime to);
        void UpdateEmailForRetry(int emailId, string errorMessage);
        void UpdateEmailStatus(int emailId, EmailStatus status, string errorMessage = null, DateTime? sentOn = null);
        Task<int> GetEmailCountByStatusAsync(EmailStatus status);
        Task<Dictionary<string, int>> GetEmailStatsByCategory();
        void BulkUpdateStatus(IEnumerable<int> emailIds, EmailStatus status);
        void DeleteOldEmails(DateTime olderThan);
    }
}
