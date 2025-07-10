using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Aman;
using Microsoft.EntityFrameworkCore;
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

        // Optimized: Use AsNoTracking for read-only queries
        public IEnumerable<Aman> Amans => context.Amans
            .AsNoTracking()
            .OrderByDescending(a => a.CreationDateTime);

        // Add async methods for better performance
        public async Task<IEnumerable<Aman>> GetAmansAsync()
        {
            return await context.Amans
                .AsNoTracking()
                .OrderByDescending(a => a.CreationDateTime)
                .ToListAsync();
        }

        // Add paged query for better performance
        public async Task<(IEnumerable<Aman> items, int totalCount)> GetPagedAmansAsync(int page, int pageSize, string userRole, string userName)
        {
            var query = context.Amans.AsNoTracking();

            if (userRole != "AdminQM")
            {
                query = query.Where(x => x.Creator == userName || x.Responsible == userName || x.Verifier == userName);
            }

            var totalCount = await query.CountAsync();
            
            var items = await query
                .OrderByDescending(a => a.CreationDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // Projection for summary views
        public async Task<IEnumerable<AmanSummaryDto>> GetAmanSummariesAsync()
        {
            return await context.Amans
                .AsNoTracking()
                .Select(a => new AmanSummaryDto
                {
                    AmanID = a.AmanID,
                    Findings = a.Findings,
                    Status = a.Status,
                    CreationDateTime = a.CreationDateTime,
                    StartDate = a.StartDate,
                    EndDate = a.EndDate,
                    Priority = a.Priority,
                    Progress = a.Progress
                })
                .OrderByDescending(a => a.CreationDateTime)
                .ToListAsync();
        }

        // Optimized single item retrieval
        public async Task<Aman> GetAmanByIdAsync(string id)
        {
            return await context.Amans
                .FirstOrDefaultAsync(a => a.AmanID == id);
        }

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

        // Async version of Save
        public async Task SaveAsync(Aman aman)
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
                Aman dbEntry = await context.Amans.FirstOrDefaultAsync(a => a.AmanID == aman.AmanID);
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
            await context.SaveChangesAsync();
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

        // Async version of SaveProgress
        public async Task SaveProgressAsync(Aman aman)
        {
            Aman dbEntry = await context.Amans.FirstOrDefaultAsync(x => string.Compare(x.AmanID, aman.AmanID) == 0);
            if (dbEntry != null)
            {
                dbEntry.Progress = aman.Progress;
                if (aman.Progress == 100)
                {
                    dbEntry.ContentType = aman.ContentType;
                    dbEntry.FileName = aman.FileName;
                }
            }
            await context.SaveChangesAsync();
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

        // Optimized reschedules query
        public IEnumerable<Reschedule> GetReschedules(string AmanID)
        {
            return context.Reschedules
                .AsNoTracking()
                .Where(r => string.Compare(r.AmanID, AmanID) == 0)
                .OrderByDescending(r => r.RescheduleID);
        }

        public async Task<IEnumerable<Reschedule>> GetReschedulesAsync(string AmanID)
        {
            return await context.Reschedules
                .AsNoTracking()
                .Where(r => string.Compare(r.AmanID, AmanID) == 0)
                .OrderByDescending(r => r.RescheduleID)
                .ToListAsync();
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

        // Bulk operations for better performance
        public async Task BulkInsertAsync(IEnumerable<Aman> amans)
        {
            context.Amans.AddRange(amans);
            await context.SaveChangesAsync();
        }

        public async Task BulkUpdateAsync(IEnumerable<Aman> amans)
        {
            context.Amans.UpdateRange(amans);
            await context.SaveChangesAsync();
        }
    }

    // DTO for projection queries
    public class AmanSummaryDto
    {
        public string AmanID { get; set; }
        public string Findings { get; set; }
        public int Status { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Priority { get; set; }
        public int Progress { get; set; }
    }
}
