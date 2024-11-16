using Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class EFSemarDownloadRepository : ISemarDownloadRepository
    {
        private ApplicationDbContext context;
        public EFSemarDownloadRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<SemarDownloadHistory> SemarDownloadHistories => context.SemarDownloadHistories;

        public void Save(SemarDownloadHistory history)
        {
            context.SemarDownloadHistories.Add(history);
            context.SaveChanges();
        }
    }
}
