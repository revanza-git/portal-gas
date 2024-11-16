using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IHSSEReportRepository
    {
        IEnumerable<HSSEReport> HSSEReports { get; }
        void Save(HSSEReport report, String mode);
        void Delete(HSSEReport report);
        String GetNextID();
    }
}
