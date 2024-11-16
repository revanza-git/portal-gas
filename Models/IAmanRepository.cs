using System;
using System.Collections.Generic;

namespace Admin.Models
{
    public interface IAmanRepository
    {
        IEnumerable<Aman> Amans { get; }
        void Delete(Aman aman);
        void Save(Aman aman);
        Aman Approve(String AmanID);
        Aman Close(String AmanID);
        void SaveProgress(Aman aman);

        IEnumerable<Reschedule> GetReschedules(String AmanID);
        void SaveReschedule(Reschedule reschedule);
        void ApproveReschedule(Reschedule reschedule);
        void RejectReschedule(Reschedule reschedule);
        void UpdateOverdueNotif(String AmanID);
    }

}