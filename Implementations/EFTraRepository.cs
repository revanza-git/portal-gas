using Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class EFTraRepository : ITraRepository
    {
        private ApplicationDbContext context;
        public EFTraRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Tra> Tras => context.Tras;

        public IEnumerable<Project> GetProjects(String VendorID)
        {
            return context.Projects.Where(x => x.VendorID == VendorID);
        }

        public IEnumerable<Worker> GetWorkers(String TraID)
        {
            return context.Workers.Where(x => x.TraID == TraID);
        }

        public Worker GetWorker(String WorkerID)
        {
            return context.Workers.FirstOrDefault(x => x.WorkerID == WorkerID);
        }

        public IEnumerable<ProjectTask> GetProjectTasks(String TraID)
        {
            return context.ProjectTasks.Where(x => x.TraID == TraID);
        }

        public ProjectTask GetProjectTask(String ProjectTaskID)
        {
            return context.ProjectTasks.FirstOrDefault(x => x.ProjectTaskID == ProjectTaskID);
        }

        public String GetVendorName(String VendorID)
        {
            Vendor vendor = context.Vendors.FirstOrDefault(x => x.VendorID == VendorID);
            if (vendor != null)
                return vendor.VendorName;
            else
                return VendorID;
        }

        public String GetProjectName(String ProjectID)
        {
            Project project = context.Projects.FirstOrDefault(x => x.ProjectID == ProjectID);
            if (project != null)
                return project.ProjectName;
            else
                return ProjectID;
        }

        public void Save(Tra tra)
        {
            if (tra.TraID == null)
            {
                tra.TraID = GetNextID();
                tra.Status = 0;
                tra.Date = DateTime.Now;
                context.Tras.Add(tra);
            }
            else
            {
                Tra search = context.Tras.FirstOrDefault(x => x.TraID == tra.TraID);
                if (search != null)
                {

                }
                else
                {
                    context.Tras.Add(tra);
                }
            }
            context.SaveChanges();
        }

        public void UpdateStatus(String TraID)
        {
            Tra search = context.Tras.FirstOrDefault(x => x.TraID == TraID);
            if (search != null)
            {
                switch(search.Status)
                {
                    case 1:
                        search.SponsorPekerjaan = context.Projects.FirstOrDefault(x => x.ProjectID == search.Project).SponsorPekerjaan;
                        break;
                    case 2:
                        search.HSSE = context.Projects.FirstOrDefault(x => x.ProjectID == search.Project).HSSE;
                        break;
                    case 3:
                        search.PimpinanPemilikWilayah = context.Projects.FirstOrDefault(x => x.ProjectID == search.Project).PemilikWilayah;
                        break;
                }
                search.Status = search.Status + 1;
            }
            context.SaveChanges();
        }

        public void SaveTask(ProjectTask task)
        {
            if (task.ProjectTaskID == null)
            {
                task.ProjectTaskID = GetTaskNextID(task.TraID);
                context.ProjectTasks.Add(task);
            }
            else
            {
                ProjectTask search = context.ProjectTasks.FirstOrDefault(x => x.ProjectTaskID == task.ProjectTaskID);
                if (search != null)
                {
                    search.SequenceOfBasicJobSteps = task.SequenceOfBasicJobSteps;
                    search.Consequence = task.Consequence;
                    search.Hazard = task.Hazard;
                    search.InitialRisk = task.InitialRisk;
                    search.RecommendedAction = task.RecommendedAction;
                    search.RoleResponsibility = task.RoleResponsibility;
                    search.ResidualRisk = task.ResidualRisk;
                    search.ALARP = task.ALARP;
                    search.AC = task.AC;
                }
                else
                {
                    context.ProjectTasks.Add(task);
                }
            }
            context.SaveChanges();
        }

        public void SaveWorker(Worker worker)
        {
            if (worker.WorkerID == null)
            {
                worker.WorkerID = GetWorkerNextID(worker.TraID);
                context.Workers.Add(worker);
            }
            else
            {
                Worker search = context.Workers.FirstOrDefault(x => x.WorkerID == worker.WorkerID);
                if (search != null)
                {
                    search.WorkerName = worker.WorkerName;
                }
                else
                {
                    context.Workers.Add(worker);
                }
            }
            context.SaveChanges();
        }

        private String GetNextID()
        {
            String ID;
            try
            {
                Tra LastRecord = context.Tras.Where(x => x.TraID.Contains("TRA")).OrderByDescending(x => x.TraID).First(x => x.TraID.Contains("TRA"));
                String LastID = LastRecord.TraID.Substring(3);

                int NextID = Int32.Parse(LastID) + 1;
                ID = "TRA" + NextID.ToString().PadLeft(4, '0');
            }
            catch (Exception)
            {
                ID = "TRA0001";
            }
            return (ID);
        }

        private String GetWorkerNextID(String TraID)
        {
            String ID;
            try
            {
                Worker LastRecord = context.Workers.OrderByDescending(x => x.TraID).Where(x => x.WorkerID.ToString().StartsWith(TraID)).First();
                String LastID = LastRecord.WorkerID.Substring(TraID.Length + 1);

                int NextID = Int32.Parse(LastID) + 1;
                ID = TraID + "W" + NextID.ToString().PadLeft(2, '0');
            }
            catch (Exception)
            {
                ID = TraID + "W01";
            }
            return (ID);
        }

        private String GetTaskNextID(String TraID)
        {
            String ID;
            try
            {
                ProjectTask LastRecord = context.ProjectTasks.OrderByDescending(x => x.TraID).Where(x => x.ProjectTaskID.ToString().StartsWith(TraID)).First();
                String LastID = LastRecord.ProjectTaskID.Substring(TraID.Length + 1);

                int NextID = Int32.Parse(LastID) + 1;
                ID = TraID + "T" + NextID.ToString().PadLeft(2, '0');
            }
            catch (Exception)
            {
                ID = TraID + "T01";
            }
            return (ID);
        }

        public IEnumerable<TraStatus> GetTraStatuses()
        {
            return new List<TraStatus>
            {
                new TraStatus() { TraStatusID=0, Deskripsi="Pending"},
                new TraStatus() { TraStatusID=1, Deskripsi="Menunggu Review Sponsor"},
                new TraStatus() { TraStatusID=2, Deskripsi="Menunggu Review HSSE"},
                new TraStatus() { TraStatusID=3, Deskripsi="Menunggu Persetujuan Pemilik Wilayah"},
                new TraStatus() { TraStatusID=4, Deskripsi="Selesai"}
            };
        }
    }
}
