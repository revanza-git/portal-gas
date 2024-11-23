using Admin.Models.Tra;
using Admin.Models.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface ITraRepository
    {
        IEnumerable<Tra> Tras { get; }
        string GetVendorName(string VendorID);
        string GetProjectName(string ProjectID);
        IEnumerable<Project> GetProjects(string VendorID);
        IEnumerable<ProjectTask> GetProjectTasks(string TraID);
        IEnumerable<Worker> GetWorkers(string TraID);
        Worker GetWorker(string WorkerID);
        ProjectTask GetProjectTask(string ProjectTaskID);
        IEnumerable<TraStatus> GetTraStatuses();
        void Save(Tra tra);
        void UpdateStatus(string TraID);
        void SaveTask(ProjectTask task);
        void SaveWorker(Worker worker);
        //String GetNextID();
    }
}
