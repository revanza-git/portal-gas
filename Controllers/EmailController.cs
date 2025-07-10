using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Admin.Interfaces.Repositories;
using Admin.Models.User;
using Admin.Services;
using System;

namespace Admin.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailRepository _repository;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(
            IEmailRepository repo,
            IEmailService emailService,
            ILogger<EmailController> logger)
        {
            _repository = repo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> Send()
        {
            try
            {
                _logger.LogInformation("Starting manual email send process");
                
                await _emailService.ProcessEmailQueueAsync();
                
                _logger.LogInformation("Manual email send process completed");
                return Ok(new { success = true, message = "Emails processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during manual email send process");
                return StatusCode(500, new { success = false, message = "Error processing emails", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Stats(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var stats = await _emailService.GetDeliveryStatsAsync(from, to);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email statistics");
                return StatusCode(500, new { success = false, message = "Error retrieving statistics", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            try
            {
                var pendingEmails = await _emailService.GetPendingEmailsAsync();
                return Ok(pendingEmails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending emails");
                return StatusCode(500, new { success = false, message = "Error retrieving pending emails", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Retry(int emailId)
        {
            try
            {
                var result = await _emailService.RetryFailedEmailAsync(emailId);
                if (result)
                {
                    return Ok(new { success = true, message = "Email scheduled for retry" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to schedule email for retry" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrying email {emailId}");
                return StatusCode(500, new { success = false, message = "Error retrying email", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendTemplated([FromBody] TemplatedEmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _emailService.SendTemplatedEmailAsync(
                    request.TemplateType,
                    request.Recipient,
                    request.Data,
                    request.Language ?? "en",
                    request.Priority,
                    request.Category);

                if (result)
                {
                    return Ok(new { success = true, message = "Templated email created successfully" });
                }
                else
                {
                    return BadRequest(new { success = false, message = "Failed to create templated email" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating templated email");
                return StatusCode(500, new { success = false, message = "Error creating templated email", error = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> CleanupOldEmails(int daysOld = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysOld);
                _repository.DeleteOldEmails(cutoffDate);
                
                _logger.LogInformation($"Cleaned up emails older than {daysOld} days");
                return Ok(new { success = true, message = $"Cleaned up emails older than {daysOld} days" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old emails");
                return StatusCode(500, new { success = false, message = "Error cleaning up old emails", error = ex.Message });
            }
        }
    }

    public class TemplatedEmailRequest
    {
        public string TemplateType { get; set; }
        public string Recipient { get; set; }
        public object Data { get; set; }
        public string Language { get; set; } = "en";
        public EmailPriority Priority { get; set; } = EmailPriority.Medium;
        public string Category { get; set; }
    }
}
