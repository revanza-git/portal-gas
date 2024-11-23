using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFLocationRepository : ILocationRepository
    {
        private ApplicationDbContext context;
        public EFLocationRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Location> Locations => context.Locations;

        public void Save(Location location)
        {
            if (location.LocationID == 0)
            {
                context.Locations.Add(location);
            }
            else
            {
                Location search = context.Locations.FirstOrDefault(x => x.LocationID == location.LocationID);
                if (search != null)
                {
                    search.Deskripsi = location.Deskripsi;
                }
            }
            context.SaveChanges();
        }

        public void Delete(Location location)
        {
            context.Locations.Remove(location);
            context.SaveChanges();
        }
    }
}
