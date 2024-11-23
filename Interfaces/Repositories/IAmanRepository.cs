using Admin.Models.Aman;
using System;
using System.Collections.Generic;

namespace Admin.Interfaces.Repositories
{
    public interface IAmanRepository
    {
        IEnumerable<Aman> Amans { get; }
        void Delete(Aman aman);
        void Save(Aman aman);
        Aman Approve(string AmanID);
        Aman Close(string AmanID);
        void SaveProgress(Aman aman);

        IEnumerable<Reschedule> GetReschedules(string AmanID);
        void SaveReschedule(Reschedule reschedule);
        void ApproveReschedule(Reschedule reschedule);
        void RejectReschedule(Reschedule reschedule);
        void UpdateOverdueNotif(string AmanID);
    }

}