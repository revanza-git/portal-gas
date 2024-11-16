using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface ISemarDownloadRepository
    {
        IEnumerable<SemarDownloadHistory> SemarDownloadHistories { get; }
        void Save(SemarDownloadHistory history);
    }
}
