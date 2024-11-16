using Admin.Data;
using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class SemarService : ISemarService
    {
        private ApplicationDbContext context;
        public SemarService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Semar> Semars => context.Semars;
    }
}
