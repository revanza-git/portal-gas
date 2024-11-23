using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFProjectRepository : IProjectRepository
    {
        private ApplicationDbContext context;
        public EFProjectRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Project> Projects => context.Projects;

        public void Save(Project project)
        {
            if (project.status == 0)
            {
                context.Projects.Add(project);
            }
            else
            {
                Project search = context.Projects.FirstOrDefault(x => x.ProjectID == project.ProjectID);
                if (search != null)
                {
                    search.status = project.status;
                }
            }
            context.SaveChanges();
        }
    }
}
