using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IDCUDownloadRepository
    {
        IEnumerable<DCUDownloadHistory> DCUDownloadHistories { get; }
        void Save(DCUDownloadHistory history);
    }
}
