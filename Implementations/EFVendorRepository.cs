using Admin.Data;
using Admin.Helpers;
using Admin.Models;
using Admin.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Implementations
{
    public class EFVendorRepository : IVendorRepository
    {
        private readonly ApplicationDbContext context;
        private readonly ApiHelper apiHelper;

        public EFVendorRepository(ApplicationDbContext ctx, ApiHelper apiHelper)
        {
            context = ctx;
            this.apiHelper = apiHelper;
        }

        public IEnumerable<Vendor> Vendors => context.Vendors;

        public async Task SaveAsync(Vendor vendor)
        {
            if (vendor.VendorID == null)
            {
                vendor.VendorID = GetVendorNextID();
                vendor.Status = 1;
                var res = await apiHelper.AddVendorAsync(vendor.VendorID, vendor.VendorName, vendor.Email, "NewVendor123#@!");
                if (res.Trim() == "OK")
                {
                    context.Vendors.Add(vendor);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                var search = context.Vendors.FirstOrDefault(x => x.VendorID == vendor.VendorID);
                if (search != null)
                {
                    search.Email = vendor.Email;
                    search.VendorName = vendor.VendorName;
                    search.Status = vendor.Status;
                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task SaveProjectAsync(Project project)
        {
            if (project.ProjectID == null)
            {
                project.ProjectID = GetProjectNextID();
                var res = await apiHelper.AddProjectAsync(project);
                if (res.Trim() == "OK")
                {
                    context.Projects.Add(project);
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                var search = context.Projects.FirstOrDefault(x => x.ProjectID == project.ProjectID);
                if (search != null)
                {
                    search.ProjectName = project.ProjectName;
                    search.SponsorPekerjaan = project.SponsorPekerjaan;
                    search.PemilikWilayah = project.PemilikWilayah;
                    await context.SaveChangesAsync();
                }
            }
        }

        public IEnumerable<Project> GetProjects(string vendorID)
        {
            return context.Projects.Where(x => x.VendorID == vendorID);
        }

        public Project GetProject(string projectID)
        {
            return context.Projects.FirstOrDefault(x => x.ProjectID == projectID);
        }

        private string GetVendorNextID()
        {
            string id;
            try
            {
                var lastRecord = context.Vendors.OrderByDescending(x => x.VendorID).First();
                var lastID = lastRecord.VendorID.Substring(1);

                var nextID = int.Parse(lastID) + 1;
                id = "V" + nextID.ToString().PadLeft(3, '0');
            }
            catch (Exception)
            {
                id = "V001";
            }
            return id;
        }

        private string GetProjectNextID()
        {
            string id;
            try
            {
                var lastRecord = context.Projects.OrderByDescending(x => x.ProjectID).First();
                var lastID = lastRecord.ProjectID.Substring(3);

                var nextID = int.Parse(lastID) + 1;
                id = "PRO" + nextID.ToString().PadLeft(4, '0');
            }
            catch (Exception)
            {
                id = "PRO0001";
            }

            return id;
        }
    }
}

