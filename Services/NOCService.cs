using Admin.Data;
using Admin.Interfaces.Services;
using Admin.Models.NOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class NOCService : INOCService
    {
        private ApplicationDbContext context;
        public NOCService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<NOC> NOC => context.NOCs;

       
    }
}
