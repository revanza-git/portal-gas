using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.DCU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFDCUDownloadRepository : IDCUDownloadRepository
    {
        private ApplicationDbContext context;
        public EFDCUDownloadRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<DCUDownloadHistory> DCUDownloadHistories => context.DCUDownloadHistories;

        public void Save(DCUDownloadHistory history)
        {
            context.DCUDownloadHistories.Add(history);
            context.SaveChanges();
        }
    }
}
