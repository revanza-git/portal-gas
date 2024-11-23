using Admin.Data;
using Admin.Interfaces.Services;
using Admin.Models.Overtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class SDMService : ISDMService
    {
        private ApplicationDbContext context;
        public SDMService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Overtime> Overtimes => context.Overtime;
    }
}
