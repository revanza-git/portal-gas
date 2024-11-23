using Admin.Models.DCU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IDCUDownloadRepository
    {
        IEnumerable<DCUDownloadHistory> DCUDownloadHistories { get; }
        void Save(DCUDownloadHistory history);
    }
}
