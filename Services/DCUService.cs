using Admin.Data;
using Admin.Interfaces.Services;
using Admin.Models.DCU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class DCUService : IDCUService
    {
        private ApplicationDbContext context;
        public DCUService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<DCU> DCU => context.DCUs;
    }
}
