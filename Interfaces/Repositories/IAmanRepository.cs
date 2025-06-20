using Admin.Models.Aman;
using Admin.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // Phase 2 Optimization: Async methods
        Task<IEnumerable<Aman>> GetAmansAsync();
        Task<(IEnumerable<Aman> items, int totalCount)> GetPagedAmansAsync(int page, int pageSize, string userRole, string userName);
        Task<IEnumerable<AmanSummaryDto>> GetAmanSummariesAsync();
        Task<Aman> GetAmanByIdAsync(string id);
        Task SaveAsync(Aman aman);
        Task SaveProgressAsync(Aman aman);
        Task<IEnumerable<Reschedule>> GetReschedulesAsync(string AmanID);
        Task BulkInsertAsync(IEnumerable<Aman> amans);
        Task BulkUpdateAsync(IEnumerable<Aman> amans);
    }
}