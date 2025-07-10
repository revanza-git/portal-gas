using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Admin.Services;
using Admin.Interfaces.Repositories;
using Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin,AtasanAdmin")]
    public class EmailAdminController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IEmailRepository _emailRepository;
        private readonly ILogger<EmailAdminController> _logger;

        public EmailAdminController(
            IEmailService emailService,
            IEmailRepository emailRepository,
            ILogger<EmailAdminController> logger)
        {
            _emailService = emailService;
            _emailRepository = emailRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new EmailAdminViewModel();
                
                // Get statistics
                viewModel.Stats = await _emailService.GetDeliveryStatsAsync();
                
                // Get pending emails
                viewModel.PendingEmails = (await _emailService.GetPendingEmailsAsync()).Take(10).ToList();
                
                // Get recent emails (last 20)
                viewModel.RecentEmails = _emailRepository.Emails
                    .OrderByDescending(e => e.CreatedOn)
                    .Take(20)
                    .ToList();

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading email admin dashboard");
                TempData["Error"] = "Error loading dashboard: " + ex.Message;
                return View(new EmailAdminViewModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Stats(DateTime? from = null, DateTime? to = null)
        {
            try
            {
                var stats = await _emailService.GetDeliveryStatsAsync(from, to);
                return Json(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving email statistics");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendingEmails()
        {
            try
            {
                var pendingEmails = await _emailService.GetPendingEmailsAsync();
                return Json(pendingEmails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending emails");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RetryEmail(int emailId)
        {
            try
            {
                var result = await _emailService.RetryFailedEmailAsync(emailId);
                if (result)
                {
                    TempData["Success"] = "Email scheduled for retry successfully";
                    return Json(new { success = true, message = "Email scheduled for retry" });
                }
                else
                {
                    TempData["Error"] = "Failed to schedule email for retry";
                    return Json(new { success = false, message = "Failed to schedule email for retry" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrying email {emailId}");
                TempData["Error"] = "Error retrying email: " + ex.Message;
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessQueue()
        {
            try
            {
                await _emailService.ProcessEmailQueueAsync();
                TempData["Success"] = "Email queue processed successfully";
                return Json(new { success = true, message = "Email queue processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing email queue");
                TempData["Error"] = "Error processing email queue: " + ex.Message;
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CleanupOldEmails(int daysOld = 30)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysOld);
                _emailRepository.DeleteOldEmails(cutoffDate);
                
                TempData["Success"] = $"Cleaned up emails older than {daysOld} days";
                return Json(new { success = true, message = $"Cleaned up emails older than {daysOld} days" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old emails");
                TempData["Error"] = "Error cleaning up old emails: " + ex.Message;
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> EmailDetails(int id)
        {
            try
            {
                var email = _emailRepository.GetById(id);
                if (email == null)
                {
                    return NotFound();
                }
                
                return Json(email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving email details for ID {id}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailRequest request)
        {
            try
            {
                var result = await _emailService.SendTemplatedEmailAsync(
                    request.TemplateType,
                    request.Recipient,
                    request.TestData,
                    "en",
                    EmailPriority.Low,
                    "TEST"
                );

                if (result)
                {
                    return Json(new { success = true, message = "Test email created successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to create test email" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    public class EmailAdminViewModel
    {
        public EmailDeliveryStats Stats { get; set; } = new EmailDeliveryStats();
        public List<Email> PendingEmails { get; set; } = new List<Email>();
        public List<Email> RecentEmails { get; set; } = new List<Email>();
    }

    public class TestEmailRequest
    {
        public string TemplateType { get; set; }
        public string Recipient { get; set; }
        public object TestData { get; set; }
    }
} 