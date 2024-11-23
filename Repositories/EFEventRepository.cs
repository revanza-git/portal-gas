using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFEventRepository : IEventRepository
    {
        private ApplicationDbContext context;
        public EFEventRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IEnumerable<Event> Events => context.Events;
    }
}
