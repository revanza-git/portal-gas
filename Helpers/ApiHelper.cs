using Admin.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Admin.Helpers
{
    public class ApiHelper
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient client;

        public ApiHelper(IConfiguration configuration, HttpClient client)
        {
            this.configuration = configuration;
            this.client = client;
            this.client.DefaultRequestHeaders.Add("API-Key", configuration["API:Key"]);
            this.client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        }

        public async Task<string> AddVendorAsync(string userName, string name, string email, string password)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("UserName", userName),
                new KeyValuePair<string, string>("Name", name),
                new KeyValuePair<string, string>("Email", email),
                new KeyValuePair<string, string>("Password", password)
            };
            var content = new FormUrlEncodedContent(postData);

            var response = await client.PostAsync(configuration["API:Host"] + "/Account/AddVendor", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddProjectAsync(Project project)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ProjectID", project.ProjectID),
                new KeyValuePair<string, string>("VendorID", project.VendorID),
                new KeyValuePair<string, string>("ProjectName", project.ProjectName),
                new KeyValuePair<string, string>("PemilikWilayah", project.PemilikWilayah),
                new KeyValuePair<string, string>("SponsorPekerjaan", project.SponsorPekerjaan),
                new KeyValuePair<string, string>("HSSE", project.HSSE),
                new KeyValuePair<string, string>("status", project.status.ToString())
            };
            var content = new FormUrlEncodedContent(postData);

            var response = await client.PostAsync(configuration["API:Host"] + "/Project/Add", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetReportsAsync()
        {
            try
            {
                var response = await client.GetAsync(configuration["API:Host"] + "/HSSEReport/GetReports");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public async Task<string> GetReportAsync(string id)
        {
            try
            {
                var response = await client.GetAsync(configuration["API:Host"] + "/HSSEReport/GetReport/" + id);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public async Task<string> UpdateStatusAsync(string id)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Id", id)
            };
            var content = new FormUrlEncodedContent(postData);

            var response = await client.PostAsync(configuration["API:Host"] + "/HSSEReport/UpdateStatus", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task DownloadFileAsync(string fileName)
        {
            var uploadPath = configuration["UploadPath:hsse"];
            using (var contentStream = await client.GetStreamAsync(configuration["API:Host"] + "/HSSEReport/GetFile?FileName=" + fileName))
            using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
            {
                await contentStream.CopyToAsync(fileStream);
            }
        }

        public async Task<string> GetTrasAsync()
        {
            try
            {
                var response = await client.GetAsync(configuration["API:Host"] + "/Tra/GetTras");
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public async Task<string> GetTraAsync(string id)
        {
            try
            {
                var response = await client.GetAsync(configuration["API:Host"] + "/Tra/GetTra/" + id);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public async Task<string> GetProjectTasksAsync(string id)
        {
            try
            {
                var response = await client.GetAsync(configuration["API:Host"] + "/Tra/GetProjectTasks/" + id);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public async Task<string> GetWorkersAsync(string id)
        {
            try
            {
                var response = await client.GetAsync(configuration["API:Host"] + "/Tra/GetWorkers/" + id);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "[]";
            }
        }

        public async Task<string> UpdateStatusTraAsync(string id)
        {
            try
            {
                var postData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Id", id)
                };
                var content = new FormUrlEncodedContent(postData);

                var response = await client.PostAsync(configuration["API:Host"] + "/Tra/UpdateStatus", content);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public async Task<string> SendEmailAsync()
        {
            var response = await client.GetAsync(configuration["Email:SenderUrl"]);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
