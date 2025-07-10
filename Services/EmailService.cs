using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Admin.Models.User;
using Admin.Interfaces.Repositories;
using Admin.Services;
using Newtonsoft.Json;
using System.Threading;

namespace Admin.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(Email email);
        Task<bool> SendTemplatedEmailAsync(string templateType, string recipient, object data, string language = "en", EmailPriority priority = EmailPriority.Medium, string category = null);
        Task ProcessEmailQueueAsync(CancellationToken cancellationToken = default);
        Task<bool> ValidateEmailAddress(string email);
        Task<List<Email>> GetPendingEmailsAsync();
        Task<bool> RetryFailedEmailAsync(int emailId);
        Task<EmailDeliveryStats> GetDeliveryStatsAsync(DateTime? from = null, DateTime? to = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailTemplateService _templateService;
        private readonly EmailValidationAttribute _emailValidator;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger,
            IEmailRepository emailRepository,
            IEmailTemplateService templateService)
        {
            _configuration = configuration;
            _logger = logger;
            _emailRepository = emailRepository;
            _templateService = templateService;
            _emailValidator = new EmailValidationAttribute();
        }

        public async Task<bool> SendEmailAsync(Email email)
        {
            try
            {
                _logger.LogInformation($"Attempting to send email {email.EmailID} to {email.Receiver}");

                if (!await ValidateEmailAddress(email.Receiver))
                {
                    _logger.LogWarning($"Invalid email address: {email.Receiver}");
                    await UpdateEmailStatus(email.EmailID, EmailStatus.Failed, "Invalid email address");
                    return false;
                }

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"], 
                    _configuration["Email:FromEmail"]));
                emailMessage.To.Add(new MailboxAddress("", email.Receiver));
                emailMessage.Subject = email.Subject;
                emailMessage.Body = new TextPart("html") { Text = email.Message };

                using (var client = new SmtpClient())
                {
                    var smtpServer = _configuration["Email:SmtpServer"];
                    var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                    
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                    
                    var smtpUser = _configuration["Email:SmtpUser"];
                    var smtpPassword = _configuration["Email:SmtpPassword"];
                    
                    if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
                    {
                        await client.AuthenticateAsync(smtpUser, smtpPassword);
                    }
                    
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }

                await UpdateEmailStatus(email.EmailID, EmailStatus.Sent);
                _logger.LogInformation($"Email {email.EmailID} sent successfully to {email.Receiver}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email {email.EmailID} to {email.Receiver}");
                
                // Implement retry logic
                if (email.RetryCount < email.MaxRetries)
                {
                    await UpdateEmailForRetry(email.EmailID, ex.Message);
                    _logger.LogInformation($"Email {email.EmailID} scheduled for retry ({email.RetryCount + 1}/{email.MaxRetries})");
                }
                else
                {
                    await UpdateEmailStatus(email.EmailID, EmailStatus.Failed, ex.Message);
                    _logger.LogError($"Email {email.EmailID} failed permanently after {email.MaxRetries} retries");
                }
                
                return false;
            }
        }

        public async Task<bool> SendTemplatedEmailAsync(string templateType, string recipient, object data, string language = "en", EmailPriority priority = EmailPriority.Medium, string category = null)
        {
            try
            {
                if (!_templateService.ValidateTemplateData(templateType, data))
                {
                    _logger.LogWarning($"Invalid template data for template type: {templateType}");
                    return false;
                }

                var subject = await _templateService.GetSubjectAsync(templateType, data, language);
                var content = await _templateService.GenerateEmailContentAsync(templateType, data, language);
                var templateData = JsonConvert.SerializeObject(data);

                var email = new Email
                {
                    Receiver = recipient,
                    Subject = subject,
                    Message = content,
                    TemplateType = templateType,
                    TemplateData = templateData,
                    Priority = (int)priority,
                    Category = category ?? templateType.Split('_')[0], // Extract category from template type
                    Status = (int)EmailStatus.Pending,
                    Schedule = DateTime.Now,
                    CreatedOn = DateTime.Now,
                    MaxRetries = GetMaxRetriesForPriority(priority)
                };

                _emailRepository.Save(email);
                _logger.LogInformation($"Templated email created with template {templateType} for {recipient}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create templated email with template {templateType} for {recipient}");
                return false;
            }
        }

        public async Task ProcessEmailQueueAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting email queue processing");
                
                var pendingEmails = await GetPendingEmailsAsync();
                var emailsToProcess = pendingEmails
                    .Where(e => e.Schedule <= DateTime.Now)
                    .OrderBy(e => e.Priority) // Higher priority first (4 = Critical, 1 = Low)
                    .ThenBy(e => e.CreatedOn)
                    .ToList();

                _logger.LogInformation($"Found {emailsToProcess.Count} emails to process");

                var semaphore = new SemaphoreSlim(5, 5); // Limit concurrent email sending
                var tasks = emailsToProcess.Select(async email =>
                {
                    await semaphore.WaitAsync(cancellationToken);
                    try
                    {
                        await SendEmailAsync(email);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks);
                _logger.LogInformation("Email queue processing completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email queue processing");
            }
        }

        public async Task<bool> ValidateEmailAddress(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return _emailValidator.IsValid(email);
        }

        public async Task<List<Email>> GetPendingEmailsAsync()
        {
            var pendingEmails = _emailRepository.Emails
                .Where(e => e.Status == (int)EmailStatus.Pending || e.Status == (int)EmailStatus.Retry)
                .ToList();

            return pendingEmails;
        }

        public async Task<bool> RetryFailedEmailAsync(int emailId)
        {
            try
            {
                var email = _emailRepository.Emails.FirstOrDefault(e => e.EmailID == emailId);
                if (email == null)
                {
                    _logger.LogWarning($"Email with ID {emailId} not found for retry");
                    return false;
                }

                if (email.Status != (int)EmailStatus.Failed)
                {
                    _logger.LogWarning($"Email {emailId} is not in failed status, cannot retry");
                    return false;
                }

                // Reset for retry
                email.Status = (int)EmailStatus.Retry;
                email.RetryCount = 0;
                email.ErrorMessage = null;
                email.Schedule = DateTime.Now;

                _emailRepository.UpdateStatus(emailId);
                _logger.LogInformation($"Email {emailId} reset for retry");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resetting email {emailId} for retry");
                return false;
            }
        }

        public async Task<EmailDeliveryStats> GetDeliveryStatsAsync(DateTime? from = null, DateTime? to = null)
        {
            var fromDate = from ?? DateTime.Now.AddDays(-30);
            var toDate = to ?? DateTime.Now;

            var emails = _emailRepository.Emails
                .Where(e => e.CreatedOn >= fromDate && e.CreatedOn <= toDate)
                .ToList();

            return new EmailDeliveryStats
            {
                TotalEmails = emails.Count,
                SentEmails = emails.Count(e => e.Status == (int)EmailStatus.Sent),
                FailedEmails = emails.Count(e => e.Status == (int)EmailStatus.Failed),
                PendingEmails = emails.Count(e => e.Status == (int)EmailStatus.Pending),
                RetryEmails = emails.Count(e => e.Status == (int)EmailStatus.Retry),
                DeliveryRate = emails.Count > 0 ? (double)emails.Count(e => e.Status == (int)EmailStatus.Sent) / emails.Count * 100 : 0,
                CategoryStats = emails.GroupBy(e => e.Category ?? "Unknown")
                    .ToDictionary(g => g.Key, g => new CategoryStats
                    {
                        Total = g.Count(),
                        Sent = g.Count(e => e.Status == (int)EmailStatus.Sent),
                        Failed = g.Count(e => e.Status == (int)EmailStatus.Failed)
                    }),
                PeriodFrom = fromDate,
                PeriodTo = toDate
            };
        }

        private async Task UpdateEmailStatus(int emailId, EmailStatus status, string errorMessage = null)
        {
            var email = _emailRepository.Emails.FirstOrDefault(e => e.EmailID == emailId);
            if (email != null)
            {
                email.Status = (int)status;
                email.ErrorMessage = errorMessage;
                
                if (status == EmailStatus.Sent)
                {
                    email.SentOn = DateTime.Now;
                }
                
                _emailRepository.UpdateStatus(emailId);
            }
        }

        private async Task UpdateEmailForRetry(int emailId, string errorMessage)
        {
            var email = _emailRepository.Emails.FirstOrDefault(e => e.EmailID == emailId);
            if (email != null)
            {
                email.Status = (int)EmailStatus.Retry;
                email.RetryCount++;
                email.ErrorMessage = errorMessage;
                email.Schedule = DateTime.Now.AddMinutes(GetRetryDelayMinutes(email.RetryCount));
                
                _emailRepository.UpdateStatus(emailId);
            }
        }

        private int GetMaxRetriesForPriority(EmailPriority priority)
        {
            return priority switch
            {
                EmailPriority.Critical => 5,
                EmailPriority.High => 4,
                EmailPriority.Medium => 3,
                EmailPriority.Low => 2,
                _ => 3
            };
        }

        private int GetRetryDelayMinutes(int retryCount)
        {
            // Exponential backoff: 2, 4, 8, 16, 32 minutes
            return (int)Math.Pow(2, retryCount);
        }
    }

    public class EmailDeliveryStats
    {
        public int TotalEmails { get; set; }
        public int SentEmails { get; set; }
        public int FailedEmails { get; set; }
        public int PendingEmails { get; set; }
        public int RetryEmails { get; set; }
        public double DeliveryRate { get; set; }
        public Dictionary<string, CategoryStats> CategoryStats { get; set; }
        public DateTime PeriodFrom { get; set; }
        public DateTime PeriodTo { get; set; }
    }

    public class CategoryStats
    {
        public int Total { get; set; }
        public int Sent { get; set; }
        public int Failed { get; set; }
    }

    public class EmailValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return false;

            var email = value.ToString();
            
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
} 