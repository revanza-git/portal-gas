using Admin.Models.Vendors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
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

