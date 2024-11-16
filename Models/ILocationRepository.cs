using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface ILocationRepository
    {
        IEnumerable<Location> Locations { get; }
        void Save(Location location);
        void Delete(Location location);
    }
}
