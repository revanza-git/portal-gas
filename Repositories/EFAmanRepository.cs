using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Aman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFAmanRepository : IAmanRepository
    {
        private ApplicationDbContext context;
        public EFAmanRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IEnumerable<Aman> Amans => context.Amans;

        public void Delete(Aman aman)
        {
            context.Amans.Remove(aman);
            context.SaveChanges();
        }

        public void Save(Aman aman)
        {
            if (aman.AmanID == null)
            {
                aman.AmanID = GetNextID();
                aman.Status = 2;
                aman.CreationDateTime = DateTime.Now;
                aman.ClosingDate = null;
                context.Amans.Add(aman);
            }
            else
            {
                Aman dbEntry = context.Amans.FirstOrDefault(a => a.AmanID == aman.AmanID);
                if (dbEntry != null)
                {
                    dbEntry.StartDate = aman.StartDate;
                    dbEntry.Source = aman.Source;
                    dbEntry.Location = aman.Location;
                    dbEntry.Findings = aman.Findings;
                    dbEntry.Recommendation = aman.Recommendation;
                    dbEntry.Responsible = aman.Responsible;
                    dbEntry.Verifier = aman.Verifier;
                    dbEntry.Priority = aman.Priority;
                    dbEntry.Auditors = aman.Auditors;
                    dbEntry.Classification = aman.Classification;
                    dbEntry.CorrectionType = aman.CorrectionType;
                }
            }
            context.SaveChanges();
        }

        public void SaveProgress(Aman aman)
        {
            Aman dbEntry = context.Amans.FirstOrDefault(x => string.Compare(x.AmanID, aman.AmanID) == 0);
            if (dbEntry != null)
            {
                dbEntry.Progress = aman.Progress;
                if (aman.Progress == 100)
                {
                    dbEntry.ContentType = aman.ContentType;
                    dbEntry.FileName = aman.FileName;
                }
            }
            context.SaveChanges();
        }

        public Aman Approve(string AmanID)
        {
            Aman dbEntry = context.Amans.FirstOrDefault(x => string.Compare(x.AmanID, AmanID) == 0);
            if (dbEntry != null)
            {
                dbEntry.Status = 2;
                context.SaveChanges();
            }
            return dbEntry;
        }

        public Aman Close(string AmanID)
        {
            Aman dbEntry = context.Amans.FirstOrDefault(x => string.Compare(x.AmanID, AmanID) == 0);
            if (dbEntry != null)
            {
                dbEntry.Status = 3;
                dbEntry.ClosingDate = DateTime.Now;
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void UpdateOverdueNotif(string AmanID)
        {
            Aman dbEntry = context.Amans.FirstOrDefault(x => string.Compare(x.AmanID, AmanID) == 0);
            if (dbEntry != null)
            {
                dbEntry.OverdueNotif += 1;
                context.SaveChanges();
            }
        }

        private string GetNextID()
        {
            string ID;
            try
            {
                Aman LastRecord = context.Amans.OrderByDescending(x => x.AmanID).First();
                string LastID = LastRecord.AmanID.Substring(4);

                int NextID = int.Parse(LastID) + 1;
                ID = "AMAN" + NextID.ToString().PadLeft(3, '0');
            }
            catch (Exception)
            {
                ID = "AMAN001";
            }
            return ID;
        }

        public IEnumerable<Reschedule> GetReschedules(string AmanID)
        {
            return context.Reschedules.Where(r => string.Compare(r.AmanID, AmanID) == 0);
        }

        public void SaveReschedule(Reschedule reschedule)
        {
            reschedule.Status = 1;
            context.Reschedules.Add(reschedule);
            context.SaveChanges();
        }

        public void ApproveReschedule(Reschedule reschedule)
        {
            Reschedule searchReschedule = context.Reschedules.FirstOrDefault(r => r.RescheduleID == reschedule.RescheduleID);
            if (searchReschedule != null)
            {
                searchReschedule.Status = 2;
                Aman searchAman = context.Amans.FirstOrDefault(a => a.AmanID == reschedule.AmanID);
                if (searchAman != null)
                {
                    searchAman.EndDate = reschedule.NewEndDate;
                }
                context.SaveChanges();
            }
        }

        public void RejectReschedule(Reschedule reschedule)
        {
            Reschedule searchReschedule = context.Reschedules.FirstOrDefault(r => r.RescheduleID == reschedule.RescheduleID);
            if (searchReschedule != null)
            {
                searchReschedule.Status = 3;
            }
            context.SaveChanges();
        }
    }
}
