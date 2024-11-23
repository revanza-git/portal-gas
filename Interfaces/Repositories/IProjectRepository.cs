using Admin.Models.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        IEnumerable<Project> Projects { get; }
        void Save(Project project);
    }
}
