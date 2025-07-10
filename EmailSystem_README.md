# Email System Documentation - Portal Nusantara Regas

## Overview

The Portal Nusantara Regas email system has been enhanced with enterprise-level features including:
- Professional HTML email templates
- Automatic retry mechanism with exponential backoff
- Background email processing
- Priority-based email queue
- Comprehensive admin dashboard
- Email statistics and monitoring
- Template-based email generation
- Multi-language support

## Features

### 🎨 **Professional Email Templates**
- **AMAN_NEW**: New AMAN notifications with detailed data tables
- **AMAN_PROGRESS**: Progress updates with visual progress indicators
- **SEMAR_EXPIRY**: Expiry warnings with countdown alerts
- **ACCOUNT_REGISTRATION**: User registration confirmations
- **PASSWORD_RESET**: Password reset confirmations
- Multi-language support (English/Indonesian)

### 🔄 **Automatic Retry System**
- Exponential backoff retry logic (2, 4, 8, 16, 32 minutes)
- Priority-based retry limits:
  - Critical: 5 retries
  - High: 4 retries
  - Medium: 3 retries
  - Low: 2 retries

### 📊 **Admin Dashboard**
- Real-time email statistics
- Queue management
- Failed email retry functionality
- Test email sending
- Email cleanup utilities
- Auto-refresh every 30 seconds

### 🏃 **Background Processing**
- Automatic email queue processing every 5 minutes
- Concurrent email sending (max 5 simultaneous)
- Graceful shutdown handling

## Configuration

### Email Settings (appsettings.json)
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromEmail": "noreply@nusantararegas.com",
    "FromName": "Portal Nusantara Regas",
    "ProcessingIntervalMinutes": 5,
    "MaxRetries": 3,
    "EnableBackgroundProcessing": true,
    "MaxConcurrentEmails": 5,
    "CleanupOldEmailsDays": 30
  }
}
```

## Usage Examples

### 1. Send AMAN New Notification
```csharp
await _emailService.SendTemplatedEmailAsync(
    "AMAN_NEW",
    "recipient@example.com",
    new {
        AmanNumber = "AMAN-001",
        CompanyName = "PT Example",
        VesselName = "MV Example",
        ETA = DateTime.Now.AddDays(1),
        Purpose = "Loading Operation"
    },
    "en", // Language
    EmailPriority.High,
    "AMAN"
);
```

### 2. Send Progress Update
```csharp
await _emailService.SendTemplatedEmailAsync(
    "AMAN_PROGRESS",
    "recipient@example.com",
    new {
        AmanNumber = "AMAN-001",
        Status = "In Progress",
        Progress = 75,
        NextStep = "Final Approval",
        EstimatedCompletion = DateTime.Now.AddHours(2)
    },
    "en",
    EmailPriority.Medium,
    "AMAN"
);
```

### 3. Send Expiry Warning
```csharp
await _emailService.SendTemplatedEmailAsync(
    "SEMAR_EXPIRY",
    "recipient@example.com",
    new {
        DocumentName = "Safety Certificate",
        ExpiryDate = DateTime.Now.AddDays(30),
        DaysRemaining = 30,
        RenewalUrl = "https://portal.example.com/renew"
    },
    "en",
    EmailPriority.Critical,
    "SEMAR"
);
```

## API Endpoints

### Email Controller
- `GET /Email` - Email management page
- `POST /Email/Send` - Send individual email
- `POST /Email/SendTemplated` - Send templated email
- `GET /Email/Pending` - Get pending emails
- `POST /Email/Retry/{id}` - Retry failed email
- `POST /Email/ProcessQueue` - Process email queue manually
- `GET /Email/Stats` - Get email statistics
- `POST /Email/Cleanup` - Cleanup old emails

### Email Admin Controller
- `GET /EmailAdmin` - Admin dashboard
- `GET /EmailAdmin/Stats` - Get statistics (JSON)
- `GET /EmailAdmin/PendingEmails` - Get pending emails (JSON)
- `POST /EmailAdmin/RetryEmail` - Retry specific email
- `POST /EmailAdmin/ProcessQueue` - Process queue manually
- `POST /EmailAdmin/CleanupOldEmails` - Cleanup old emails
- `GET /EmailAdmin/EmailDetails/{id}` - Get email details
- `POST /EmailAdmin/TestEmail` - Send test email

## Database Schema

### Enhanced Email Table
```sql
CREATE TABLE [dbo].[Emails] (
    [EmailID] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Receiver] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Status] int NOT NULL DEFAULT 0,
    [Schedule] datetime2(7) NULL,
    [CreatedOn] datetime2(7) NOT NULL,
    [SentOn] datetime2(7) NULL,
    [RetryCount] int NOT NULL DEFAULT 0,
    [MaxRetries] int NOT NULL DEFAULT 3,
    [ErrorMessage] nvarchar(max) NULL,
    [Priority] int NOT NULL DEFAULT 2,
    [Category] nvarchar(50) NULL,
    [TemplateType] nvarchar(50) NULL,
    [TemplateData] nvarchar(max) NULL
);

-- Indexes for performance
CREATE INDEX [IX_Emails_Status] ON [dbo].[Emails] ([Status]);
CREATE INDEX [IX_Emails_Category] ON [dbo].[Emails] ([Category]);
CREATE INDEX [IX_Emails_Priority] ON [dbo].[Emails] ([Priority]);
CREATE INDEX [IX_Emails_Schedule] ON [dbo].[Emails] ([Schedule]);
CREATE INDEX [IX_Emails_CreatedOn] ON [dbo].[Emails] ([CreatedOn]);
```

## Monitoring & Statistics

### Dashboard Features
- **Total Sent**: Total emails processed
- **Successful Deliveries**: Successfully sent emails
- **Pending Emails**: Emails waiting to be sent
- **Failed Deliveries**: Emails that failed to send
- **Success Rate**: Percentage of successful deliveries
- **System Status**: Background service, database, and SMTP status

### Email Statuses
- `Pending` (0): Waiting to be sent
- `Sent` (1): Successfully sent
- `Failed` (2): Failed to send
- `Scheduled` (3): Scheduled for future sending

### Priority Levels
- `Critical` (5): Highest priority, 5 retry attempts
- `High` (4): High priority, 4 retry attempts
- `Medium` (3): Normal priority, 3 retry attempts
- `Low` (2): Low priority, 2 retry attempts
- `Bulk` (1): Bulk emails, 1 retry attempt

## Troubleshooting

### Common Issues

1. **Emails Not Sending**
   - Check SMTP configuration in appsettings.json
   - Verify email credentials
   - Check if background service is running
   - Review error messages in email details

2. **High Failure Rate**
   - Check SMTP server status
   - Verify recipient email addresses
   - Review email content for spam triggers
   - Check network connectivity

3. **Slow Email Processing**
   - Increase MaxConcurrentEmails setting
   - Check database performance
   - Review email queue size
   - Monitor server resources

### Logs
Email system logs are available in:
- Application logs (ILogger)
- Email error messages (stored in database)
- Background service logs

## Migration Guide

### From Old System
1. **Database Migration**: Run `dotnet ef database update`
2. **Configuration**: Update appsettings.json with new email settings
3. **Code Updates**: Replace manual email creation with templated emails
4. **Testing**: Use admin dashboard to send test emails

### Breaking Changes
- Email model has new required fields
- EmailService interface has new methods
- Background service registration required in Startup.cs

## Best Practices

### Email Templates
- Use responsive HTML design
- Include fallback text for email clients
- Test templates across different email clients
- Keep templates consistent with brand guidelines

### Performance
- Use appropriate priority levels
- Implement email batching for bulk operations
- Monitor queue size and processing time
- Regular cleanup of old emails

### Security
- Use app passwords for Gmail SMTP
- Validate email addresses before sending
- Sanitize template data
- Implement rate limiting for external APIs

### Monitoring
- Set up alerts for high failure rates
- Monitor background service health
- Track email delivery statistics
- Regular review of failed emails

## Support

For technical support or questions about the email system:
1. Check the admin dashboard for system status
2. Review email error messages
3. Check application logs
4. Contact system administrator

---

**Version**: 2.0.0  
**Last Updated**: December 2024  
**Author**: Portal Nusantara Regas Development Team 