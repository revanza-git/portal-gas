using Admin.Models.HSSE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IHSSEReportRepository
    {
        IEnumerable<HSSEReport> HSSEReports { get; }
        void Save(HSSEReport report, string mode);
        void Delete(HSSEReport report);
        string GetNextID();
    }
}
