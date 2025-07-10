using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Admin.Models.User;

namespace Admin.Services
{
    public interface IEmailTemplateService
    {
        Task<string> GenerateEmailContentAsync(string templateType, object data, string language = "en");
        Task<string> GetSubjectAsync(string templateType, object data, string language = "en");
        bool ValidateTemplateData(string templateType, object data);
    }

    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, EmailTemplate> _templates;

        public EmailTemplateService(IConfiguration configuration)
        {
            _configuration = configuration;
            _templates = LoadTemplates();
        }

        public async Task<string> GenerateEmailContentAsync(string templateType, object data, string language = "en")
        {
            if (!_templates.ContainsKey(templateType))
                throw new ArgumentException($"Template type '{templateType}' not found");

            var template = _templates[templateType];
            var templateContent = language == "id" ? template.ContentId : template.ContentEn;

            // Replace placeholders with actual data
            var json = JsonConvert.SerializeObject(data);
            var dataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (var kvp in dataDict)
            {
                var placeholder = $"{{{{{kvp.Key}}}}}";
                templateContent = templateContent.Replace(placeholder, kvp.Value?.ToString() ?? "");
            }

            return templateContent;
        }

        public async Task<string> GetSubjectAsync(string templateType, object data, string language = "en")
        {
            if (!_templates.ContainsKey(templateType))
                throw new ArgumentException($"Template type '{templateType}' not found");

            var template = _templates[templateType];
            var subject = language == "id" ? template.SubjectId : template.SubjectEn;

            // Replace placeholders in subject
            var json = JsonConvert.SerializeObject(data);
            var dataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (var kvp in dataDict)
            {
                var placeholder = $"{{{{{kvp.Key}}}}}";
                subject = subject.Replace(placeholder, kvp.Value?.ToString() ?? "");
            }

            return subject;
        }

        public bool ValidateTemplateData(string templateType, object data)
        {
            if (!_templates.ContainsKey(templateType))
                return false;

            var template = _templates[templateType];
            var json = JsonConvert.SerializeObject(data);
            var dataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            foreach (var requiredField in template.RequiredFields)
            {
                if (!dataDict.ContainsKey(requiredField) || dataDict[requiredField] == null)
                    return false;
            }

            return true;
        }

        private Dictionary<string, EmailTemplate> LoadTemplates()
        {
            return new Dictionary<string, EmailTemplate>
            {
                ["AMAN_NEW"] = new EmailTemplate
                {
                    SubjectEn = "New AMAN Notification - {{AmanID}}",
                    SubjectId = "Notifikasi AMAN Baru - {{AmanID}}",
                    ContentEn = GetAmanNewTemplateEn(),
                    ContentId = GetAmanNewTemplateId(),
                    RequiredFields = new[] { "AmanID", "RecipientName", "StartDate", "EndDate", "Findings", "Recommendation", "Responsible", "Verifier" }
                },
                ["AMAN_PROGRESS"] = new EmailTemplate
                {
                    SubjectEn = "AMAN Progress Update - {{AmanID}}",
                    SubjectId = "Update Progress AMAN - {{AmanID}}",
                    ContentEn = GetAmanProgressTemplateEn(),
                    ContentId = GetAmanProgressTemplateId(),
                    RequiredFields = new[] { "AmanID", "RecipientName", "Progress" }
                },
                ["AMAN_OVERDUE"] = new EmailTemplate
                {
                    SubjectEn = "AMAN Overdue Notification - {{AmanID}}",
                    SubjectId = "Notifikasi AMAN Terlambat - {{AmanID}}",
                    ContentEn = GetAmanOverdueTemplateEn(),
                    ContentId = GetAmanOverdueTemplateId(),
                    RequiredFields = new[] { "AmanID", "RecipientName", "EndDate", "ResponsibleName" }
                },
                ["SEMAR_EXPIRY"] = new EmailTemplate
                {
                    SubjectEn = "SEMAR Expiry Notification - {{SemarID}}",
                    SubjectId = "Notifikasi Expired SEMAR - {{SemarID}}",
                    ContentEn = GetSemarExpiryTemplateEn(),
                    ContentId = GetSemarExpiryTemplateId(),
                    RequiredFields = new[] { "SemarID", "RecipientName", "Title", "ExpiredDate", "DaysRemaining" }
                },
                ["SEMAR_APPROVAL"] = new EmailTemplate
                {
                    SubjectEn = "SEMAR Approval Required - {{SemarID}}",
                    SubjectId = "Persetujuan SEMAR Diperlukan - {{SemarID}}",
                    ContentEn = GetSemarApprovalTemplateEn(),
                    ContentId = GetSemarApprovalTemplateId(),
                    RequiredFields = new[] { "SemarID", "RecipientName", "Title", "Type", "Owner" }
                },
                ["NOC_INCIDENT"] = new EmailTemplate
                {
                    SubjectEn = "New Incident Alert - NOC {{NOCID}}",
                    SubjectId = "Peringatan Insiden Baru - NOC {{NOCID}}",
                    ContentEn = GetNocIncidentTemplateEn(),
                    ContentId = GetNocIncidentTemplateId(),
                    RequiredFields = new[] { "NOCID", "RecipientName", "EntryDate", "Location", "Description" }
                },
                ["NEWS_APPROVAL"] = new EmailTemplate
                {
                    SubjectEn = "News Approval Required - {{NewsID}}",
                    SubjectId = "Persetujuan Berita Diperlukan - {{NewsID}}",
                    ContentEn = GetNewsApprovalTemplateEn(),
                    ContentId = GetNewsApprovalTemplateId(),
                    RequiredFields = new[] { "NewsID", "RecipientName", "Subject", "AuthorName" }
                },
                ["ACCOUNT_REGISTRATION"] = new EmailTemplate
                {
                    SubjectEn = "Account Registration Confirmation",
                    SubjectId = "Konfirmasi Pendaftaran Akun",
                    ContentEn = GetAccountRegistrationTemplateEn(),
                    ContentId = GetAccountRegistrationTemplateId(),
                    RequiredFields = new[] { "Name", "Username", "Password" }
                },
                ["PASSWORD_RESET"] = new EmailTemplate
                {
                    SubjectEn = "Password Reset Confirmation",
                    SubjectId = "Konfirmasi Reset Password",
                    ContentEn = GetPasswordResetTemplateEn(),
                    ContentId = GetPasswordResetTemplateId(),
                    RequiredFields = new[] { "Name", "Username", "Password" }
                },
                ["PASSWORD_RESET_LINK"] = new EmailTemplate
                {
                    SubjectEn = "Password Reset Request",
                    SubjectId = "Permintaan Reset Password",
                    ContentEn = GetPasswordResetLinkTemplateEn(),
                    ContentId = GetPasswordResetLinkTemplateId(),
                    RequiredFields = new[] { "Name", "ResetLink" }
                }
            };
        }

        private string GetAmanNewTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #2c3e50; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        .table th { background-color: #f2f2f2; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>New AMAN Notification</h2>
    </div>
    <div class='content'>
        <p>Dear {{RecipientName}},</p>
        <p>A new AMAN has been created with the following details:</p>
        
        <table class='table'>
            <tr><th>Field</th><th>Value</th></tr>
            <tr><td>AMAN ID</td><td>{{AmanID}}</td></tr>
            <tr><td>Start Date</td><td>{{StartDate}}</td></tr>
            <tr><td>End Date</td><td>{{EndDate}}</td></tr>
            <tr><td>Findings/Opportunities</td><td>{{Findings}}</td></tr>
            <tr><td>Recommendation</td><td>{{Recommendation}}</td></tr>
            <tr><td>Responsible</td><td>{{Responsible}}</td></tr>
            <tr><td>Verifier</td><td>{{Verifier}}</td></tr>
        </table>
        
        <p>Please log in to the Nusantara Regas Internal Portal to review this AMAN.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetAmanNewTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #2c3e50; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        .table th { background-color: #f2f2f2; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Notifikasi AMAN Baru</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{RecipientName}},</p>
        <p>Telah dibuat AMAN baru dengan detail sebagai berikut:</p>
        
        <table class='table'>
            <tr><th>Field</th><th>Nilai</th></tr>
            <tr><td>ID AMAN</td><td>{{AmanID}}</td></tr>
            <tr><td>Tanggal Mulai</td><td>{{StartDate}}</td></tr>
            <tr><td>Tanggal Berakhir</td><td>{{EndDate}}</td></tr>
            <tr><td>Temuan/Peluang</td><td>{{Findings}}</td></tr>
            <tr><td>Rekomendasi</td><td>{{Recommendation}}</td></tr>
            <tr><td>Penanggung Jawab</td><td>{{Responsible}}</td></tr>
            <tr><td>Verifikator</td><td>{{Verifier}}</td></tr>
        </table>
        
        <p>Silakan login ke Portal Internal Nusantara Regas untuk meninjau AMAN ini.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetAmanProgressTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #27ae60; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .progress-bar { width: 100%; background-color: #f0f0f0; border-radius: 5px; margin: 20px 0; }
        .progress-fill { height: 30px; background-color: #27ae60; border-radius: 5px; text-align: center; line-height: 30px; color: white; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>AMAN Progress Update</h2>
    </div>
    <div class='content'>
        <p>Dear {{RecipientName}},</p>
        <p>The progress of AMAN {{AmanID}} has been updated:</p>
        
        <div class='progress-bar'>
            <div class='progress-fill' style='width: {{Progress}}%'>{{Progress}}%</div>
        </div>
        
        <p>Please log in to the portal to review the updated progress.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetAmanProgressTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #27ae60; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .progress-bar { width: 100%; background-color: #f0f0f0; border-radius: 5px; margin: 20px 0; }
        .progress-fill { height: 30px; background-color: #27ae60; border-radius: 5px; text-align: center; line-height: 30px; color: white; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Update Progress AMAN</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{RecipientName}},</p>
        <p>Progress AMAN {{AmanID}} telah diperbarui:</p>
        
        <div class='progress-bar'>
            <div class='progress-fill' style='width: {{Progress}}%'>{{Progress}}%</div>
        </div>
        
        <p>Silakan login ke portal untuk meninjau progress yang telah diperbarui.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetAmanOverdueTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #e74c3c; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .warning { background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }
        .table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        .table th { background-color: #f2f2f2; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>AMAN Overdue Notification</h2>
    </div>
    <div class='content'>
        <p>Dear {{RecipientName}},</p>
        
        <div class='warning'>
            <strong>Warning:</strong> The following AMAN is overdue.
        </div>
        
        <table class='table'>
            <tr><th>Field</th><th>Value</th></tr>
            <tr><td>AMAN ID</td><td>{{AmanID}}</td></tr>
            <tr><td>End Date</td><td>{{EndDate}}</td></tr>
            <tr><td>Responsible</td><td>{{ResponsibleName}}</td></tr>
        </table>
        
        <p>Please take necessary action to complete this AMAN.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetAmanOverdueTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #e74c3c; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .warning { background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }
        .table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        .table th { background-color: #f2f2f2; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Notifikasi AMAN Terlambat</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{RecipientName}},</p>
        
        <div class='warning'>
            <strong>Peringatan:</strong> AMAN berikut telah terlambat.
        </div>
        
        <table class='table'>
            <tr><th>Field</th><th>Nilai</th></tr>
            <tr><td>ID AMAN</td><td>{{AmanID}}</td></tr>
            <tr><td>Tanggal Berakhir</td><td>{{EndDate}}</td></tr>
            <tr><td>Penanggung Jawab</td><td>{{ResponsibleName}}</td></tr>
        </table>
        
        <p>Silakan ambil tindakan yang diperlukan untuk menyelesaikan AMAN ini.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetSemarExpiryTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #e74c3c; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .warning { background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }
        .table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        .table th { background-color: #f2f2f2; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>SEMAR Expiry Notification</h2>
    </div>
    <div class='content'>
        <p>Dear {{RecipientName}},</p>
        
        <div class='warning'>
            <strong>Warning:</strong> The following SEMAR will expire in {{DaysRemaining}} days.
        </div>
        
        <table class='table'>
            <tr><th>Field</th><th>Value</th></tr>
            <tr><td>SEMAR ID</td><td>{{SemarID}}</td></tr>
            <tr><td>Title</td><td>{{Title}}</td></tr>
            <tr><td>Expiry Date</td><td>{{ExpiredDate}}</td></tr>
        </table>
        
        <p>Please take necessary action to renew or update this document.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetSemarExpiryTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #e74c3c; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .warning { background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }
        .table { width: 100%; border-collapse: collapse; margin: 20px 0; }
        .table th, .table td { border: 1px solid #ddd; padding: 12px; text-align: left; }
        .table th { background-color: #f2f2f2; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Notifikasi SEMAR Akan Berakhir</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{RecipientName}},</p>
        
        <div class='warning'>
            <strong>Peringatan:</strong> SEMAR berikut akan berakhir dalam {{DaysRemaining}} hari.
        </div>
        
        <table class='table'>
            <tr><th>Field</th><th>Nilai</th></tr>
            <tr><td>ID SEMAR</td><td>{{SemarID}}</td></tr>
            <tr><td>Judul</td><td>{{Title}}</td></tr>
            <tr><td>Tanggal Berakhir</td><td>{{ExpiredDate}}</td></tr>
        </table>
        
        <p>Silakan ambil tindakan yang diperlukan untuk memperpanjang atau memperbarui dokumen ini.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetSemarApprovalTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>SEMAR Approval Required</h2>
    </div>
    <div class='content'>
        <p>Dear {{RecipientName}},</p>
        <p>A new SEMAR approval is required for the following SEMAR:</p>
        
        <div class='credentials'>
            <h3>SEMAR Details:</h3>
            <p><strong>Title:</strong> {{Title}}</p>
            <p><strong>Type:</strong> {{Type}}</p>
            <p><strong>Owner:</strong> {{Owner}}</p>
        </div>
        
        <p>Please log in to the portal to review and approve this SEMAR.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetSemarApprovalTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Persetujuan SEMAR Diperlukan</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{RecipientName}},</p>
        <p>Persetujuan baru untuk SEMAR berikut diperlukan:</p>
        
        <div class='credentials'>
            <h3>Detail SEMAR:</h3>
            <p><strong>Judul:</strong> {{Title}}</p>
            <p><strong>Tipe:</strong> {{Type}}</p>
            <p><strong>Pemilik:</strong> {{Owner}}</p>
        </div>
        
        <p>Silakan login ke portal untuk meninjau dan menyetujui SEMAR ini.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetNocIncidentTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>New Incident Alert</h2>
    </div>
    <div class='content'>
        <p>Dear {{RecipientName}},</p>
        <p>A new incident has been reported in the NOC:</p>
        
        <div class='credentials'>
            <h3>Incident Details:</h3>
            <p><strong>Entry Date:</strong> {{EntryDate}}</p>
            <p><strong>Location:</strong> {{Location}}</p>
            <p><strong>Description:</strong> {{Description}}</p>
        </div>
        
        <p>Please log in to the portal to review this incident.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetNocIncidentTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Peringatan Insiden Baru</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{RecipientName}},</p>
        <p>Insiden baru telah dilaporkan di NOC:</p>
        
        <div class='credentials'>
            <h3>Detail Insiden:</h3>
            <p><strong>Tanggal Masuk:</strong> {{EntryDate}}</p>
            <p><strong>Lokasi:</strong> {{Location}}</p>
            <p><strong>Deskripsi:</strong> {{Description}}</p>
        </div>
        
        <p>Silakan login ke portal untuk meninjau insiden ini.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetNewsApprovalTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>News Approval Required</h2>
    </div>
    <div class='content'>
        <p>Dear {{RecipientName}},</p>
        <p>A new news approval is required for the following news:</p>
        
        <div class='credentials'>
            <h3>News Details:</h3>
            <p><strong>Subject:</strong> {{Subject}}</p>
            <p><strong>Author:</strong> {{AuthorName}}</p>
        </div>
        
        <p>Please log in to the portal to review and approve this news.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetNewsApprovalTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Persetujuan Berita Diperlukan</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{RecipientName}},</p>
        <p>Persetujuan baru untuk berita berikut diperlukan:</p>
        
        <div class='credentials'>
            <h3>Detail Berita:</h3>
            <p><strong>Judul:</strong> {{Subject}}</p>
            <p><strong>Penulis:</strong> {{AuthorName}}</p>
        </div>
        
        <p>Silakan login ke portal untuk meninjau dan menyetujui berita ini.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetAccountRegistrationTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Account Registration Confirmation</h2>
    </div>
    <div class='content'>
        <p>Dear {{Name}},</p>
        <p>Your account has been successfully registered in the Nusantara Regas Portal.</p>
        
        <div class='credentials'>
            <h3>Your Login Credentials:</h3>
            <p><strong>Username:</strong> {{Username}}</p>
            <p><strong>Password:</strong> {{Password}}</p>
        </div>
        
        <p>Please log in to the portal and change your password for security purposes.</p>
        <p><strong>Portal URL:</strong> http://portal.nusantararegas.com</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetAccountRegistrationTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #3498db; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Konfirmasi Pendaftaran Akun</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{Name}},</p>
        <p>Akun Anda telah berhasil didaftarkan di Portal Nusantara Regas.</p>
        
        <div class='credentials'>
            <h3>Kredensial Login Anda:</h3>
            <p><strong>Username:</strong> {{Username}}</p>
            <p><strong>Password:</strong> {{Password}}</p>
        </div>
        
        <p>Silakan login ke portal dan ubah password Anda untuk keamanan.</p>
        <p><strong>URL Portal:</strong> http://portal.nusantararegas.com</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #f39c12; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Password Reset Confirmation</h2>
    </div>
    <div class='content'>
        <p>Dear {{Name}},</p>
        <p>Your password has been successfully reset.</p>
        
        <div class='credentials'>
            <h3>Your New Login Credentials:</h3>
            <p><strong>Username:</strong> {{Username}}</p>
            <p><strong>New Password:</strong> {{Password}}</p>
        </div>
        
        <p>Please log in to the portal and change your password for security purposes.</p>
        <p><strong>Portal URL:</strong> http://portal.nusantararegas.com</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #f39c12; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .credentials { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Konfirmasi Reset Password</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{Name}},</p>
        <p>Password Anda telah berhasil direset.</p>
        
        <div class='credentials'>
            <h3>Kredensial Login Baru Anda:</h3>
            <p><strong>Username:</strong> {{Username}}</p>
            <p><strong>Password Baru:</strong> {{Password}}</p>
        </div>
        
        <p>Silakan login ke portal dan ubah password Anda untuk keamanan.</p>
        <p><strong>URL Portal:</strong> http://portal.nusantararegas.com</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetLinkTemplateEn()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #f39c12; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .reset-link { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; text-align: center; }
        .btn { display: inline-block; padding: 12px 24px; background-color: #f39c12; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }
        .btn:hover { background-color: #e67e22; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Password Reset Request</h2>
    </div>
    <div class='content'>
        <p>Dear {{Name}},</p>
        <p>You have requested a password reset. Please click the button below to reset your password:</p>
        
        <div class='reset-link'>
            <a href='{{ResetLink}}' class='btn'>Reset My Password</a>
        </div>
        
        <p>Or copy and paste this link into your browser:</p>
        <p><a href='{{ResetLink}}'>{{ResetLink}}</a></p>
        
        <p><strong>Note:</strong> This link will expire in 24 hours for security purposes.</p>
        <p>If you did not request this reset, please ignore this email and your password will remain unchanged.</p>
    </div>
    <div class='footer'>
        <p>This is an automated message from Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }

        private string GetPasswordResetLinkTemplateId()
        {
            return @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }
        .header { background-color: #f39c12; color: white; padding: 20px; text-align: center; }
        .content { padding: 20px; }
        .reset-link { background-color: #f8f9fa; border: 1px solid #dee2e6; padding: 20px; border-radius: 5px; margin: 20px 0; text-align: center; }
        .btn { display: inline-block; padding: 12px 24px; background-color: #f39c12; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; }
        .btn:hover { background-color: #e67e22; }
        .footer { background-color: #ecf0f1; padding: 20px; text-align: center; font-size: 12px; }
    </style>
</head>
<body>
    <div class='header'>
        <h2>Permintaan Reset Password</h2>
    </div>
    <div class='content'>
        <p>Kepada Yth. {{Name}},</p>
        <p>Anda telah meminta reset password. Silakan klik tombol di bawah ini untuk mengatur ulang password Anda:</p>
        
        <div class='reset-link'>
            <a href='{{ResetLink}}' class='btn'>Reset Password Saya</a>
        </div>
        
        <p>Atau salin dan tempel tautan ini ke browser Anda:</p>
        <p><a href='{{ResetLink}}'>{{ResetLink}}</a></p>
        
        <p><strong>Catatan:</strong> Tautan ini akan kedaluwarsa dalam 24 jam untuk keperluan keamanan.</p>
        <p>Jika Anda tidak meminta reset ini, silakan abaikan email ini dan password Anda akan tetap tidak berubah.</p>
    </div>
    <div class='footer'>
        <p>Ini adalah pesan otomatis dari Portal Nusantara Regas</p>
    </div>
</body>
</html>";
        }
    }

    public class EmailTemplate
    {
        public string SubjectEn { get; set; }
        public string SubjectId { get; set; }
        public string ContentEn { get; set; }
        public string ContentId { get; set; }
        public string[] RequiredFields { get; set; }
    }
} 