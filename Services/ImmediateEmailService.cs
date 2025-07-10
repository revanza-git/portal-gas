using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Admin.Services;
using Admin.Interfaces.Repositories;
using Admin.Models.User;
using Newtonsoft.Json;

namespace Admin.Services
{
    public interface IImmediateEmailService
    {
        Task<bool> SendImmediateTemplatedEmailAsync(string templateType, string recipient, object data, string language = "en", bool logToDatabase = true);
        Task<bool> SendImmediateEmailAsync(string recipient, string subject, string message, bool logToDatabase = true);
        Task<bool> TestSmtpConnectionAsync();
    }

    public class ImmediateEmailService : IImmediateEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ImmediateEmailService> _logger;
        private readonly IEmailTemplateService _templateService;
        private readonly IEmailRepository _emailRepository;

        public ImmediateEmailService(
            IConfiguration configuration,
            ILogger<ImmediateEmailService> logger,
            IEmailTemplateService templateService,
            IEmailRepository emailRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _templateService = templateService;
            _emailRepository = emailRepository;
        }

        public async Task<bool> SendImmediateTemplatedEmailAsync(string templateType, string recipient, object data, string language = "en", bool logToDatabase = true)
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

                return await SendImmediateEmailAsync(recipient, subject, content, logToDatabase, templateType, data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send immediate templated email with template {templateType} to {recipient}");
                return false;
            }
        }

        public async Task<bool> SendImmediateEmailAsync(string recipient, string subject, string message, bool logToDatabase = true)
        {
            return await SendImmediateEmailAsync(recipient, subject, message, logToDatabase, null, null);
        }

        private async Task<bool> SendImmediateEmailAsync(string recipient, string subject, string message, bool logToDatabase, string templateType, object templateData)
        {
            var emailId = 0;
            
            try
            {
                _logger.LogInformation($"Attempting to send immediate email to {recipient}");

                if (!IsValidEmail(recipient))
                {
                    _logger.LogWarning($"Invalid email address: {recipient}");
                    return false;
                }

                // Optionally log to database for tracking
                if (logToDatabase)
                {
                    var email = new Email
                    {
                        Receiver = recipient,
                        Subject = subject,
                        Message = message,
                        TemplateType = templateType,
                        TemplateData = templateData != null ? JsonConvert.SerializeObject(templateData) : null,
                        Priority = (int)EmailPriority.High,
                        Category = "IMMEDIATE",
                        Status = (int)EmailStatus.Pending,
                        Schedule = DateTime.Now,
                        CreatedOn = DateTime.Now,
                        MaxRetries = 1
                    };

                    _emailRepository.Save(email);
                    emailId = email.EmailID;
                    _logger.LogInformation($"Email logged to database with ID: {emailId} for recipient: {recipient}");
                }

                // Test SMTP connection first
                if (!await TestSmtpConnectionAsync())
                {
                    var errorMsg = "SMTP connection test failed";
                    _logger.LogError($"{errorMsg}. SMTP Server: {_configuration["Email:SmtpServer"]}:{_configuration["Email:SmtpPort"]}");
                    
                    if (logToDatabase && emailId > 0)
                    {
                        await UpdateEmailStatus(emailId, EmailStatus.Failed, errorMsg);
                    }
                    return false;
                }

                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(
                    _configuration["Email:FromName"], 
                    _configuration["Email:FromEmail"]));
                emailMessage.To.Add(new MailboxAddress("", recipient));
                emailMessage.Subject = subject;
                emailMessage.Body = new TextPart("html") { Text = message };

                using (var client = new SmtpClient())
                {
                    var smtpServer = _configuration["Email:SmtpServer"];
                    var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                    
                    // Set timeout for immediate sending
                    client.Timeout = 30000; // 30 seconds
                    
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

                if (logToDatabase && emailId > 0)
                {
                    await UpdateEmailStatus(emailId, EmailStatus.Sent);
                }

                _logger.LogInformation($"Immediate email sent successfully to {recipient}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send immediate email to {recipient}. SMTP Server: {_configuration["Email:SmtpServer"]}:{_configuration["Email:SmtpPort"]}");
                
                if (logToDatabase && emailId > 0)
                {
                    await UpdateEmailStatus(emailId, EmailStatus.Failed, ex.Message);
                }
                
                return false;
            }
        }

        public async Task<bool> TestSmtpConnectionAsync()
        {
            try
            {
                var smtpServer = _configuration["Email:SmtpServer"];
                var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                
                _logger.LogInformation($"Testing SMTP connection to {smtpServer}:{smtpPort}");

                using (var client = new SmtpClient())
                {
                    client.Timeout = 10000; // 10 seconds for connection test
                    
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                    
                    var smtpUser = _configuration["Email:SmtpUser"];
                    var smtpPassword = _configuration["Email:SmtpPassword"];
                    
                    if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
                    {
                        await client.AuthenticateAsync(smtpUser, smtpPassword);
                    }
                    
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("SMTP connection test successful");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SMTP connection test failed. Server: {_configuration["Email:SmtpServer"]}:{_configuration["Email:SmtpPort"]}");
                return false;
            }
        }

        private async Task UpdateEmailStatus(int emailId, EmailStatus status, string errorMessage = null)
        {
            try
            {
                // Use the proper UpdateEmailStatus method that handles all statuses correctly
                _emailRepository.UpdateEmailStatus(emailId, status, errorMessage, 
                    status == EmailStatus.Sent ? DateTime.Now : null);
                
                _logger.LogInformation($"Email {emailId} status updated to {status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to update email status for email ID: {emailId}");
            }
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
} 