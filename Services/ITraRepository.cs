using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface ITraRepository
    {
        IEnumerable<Tra> Tras { get; }
        String GetVendorName(String VendorID);
        String GetProjectName(String ProjectID);
        IEnumerable<Project> GetProjects(String VendorID);
        IEnumerable<ProjectTask> GetProjectTasks(String TraID);
        IEnumerable<Worker> GetWorkers(String TraID);
        Worker GetWorker(String WorkerID);
        ProjectTask GetProjectTask(String ProjectTaskID);
        IEnumerable<TraStatus> GetTraStatuses();
        void Save(Tra tra);
        void UpdateStatus(String TraID);
        void SaveTask(ProjectTask task);
        void SaveWorker(Worker worker);
        //String GetNextID();
    }
}
