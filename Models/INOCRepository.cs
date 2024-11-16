using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface INOCRepository
    {
        IEnumerable<NOC> NOCs { get; }
        Task SaveAsync(NOC noc, string mode);
        Task<IEnumerable<ObservationList>> GetObservationListsAsync();
        Task<IEnumerable<ClsrList>> GetClsrListsAsync();
        IEnumerable<NOCStatus> GetNOCStatuses();
        Task<string> GetNextIDAsync();
        Task DeleteAsync(NOC noc);
    }
}
