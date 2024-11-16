using Admin.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IVendorRepository
    {
        IEnumerable<Vendor> Vendors { get; }
        Task SaveAsync(Vendor vendor);
        Task SaveProjectAsync(Project project);
        IEnumerable<Project> GetProjects(string vendorID);
        Project GetProject(string projectID);
    }
}

