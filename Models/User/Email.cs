using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.User
{
    public class Email
    {
        public int EmailID { get; set; }
        
        [Required]
        [EmailAddress]
        public string Receiver { get; set; }
        
        [Required]
        public string Subject { get; set; }
        
        [Required]
        public string Message { get; set; }
        
        public int Status { get; set; } // 0: Pending, 1: Sent, 2: Failed, 3: Retry
        
        public DateTime Schedule { get; set; }
        
        public DateTime CreatedOn { get; set; }
        
        public DateTime? SentOn { get; set; }
        
        public int RetryCount { get; set; } = 0;
        
        public int MaxRetries { get; set; } = 3;
        
        public string ErrorMessage { get; set; }
        
        public string TemplateType { get; set; }
        
        public string TemplateData { get; set; } // JSON data for template
        
        public int Priority { get; set; } = 1; // 1: Low, 2: Medium, 3: High
        
        public string Category { get; set; } // AMAN, SEMAR, NOC, etc.
    }
    
    public enum EmailStatus
    {
        Pending = 0,
        Sent = 1,
        Failed = 2,
        Retry = 3,
        Cancelled = 4
    }
    
    public enum EmailPriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }
}
