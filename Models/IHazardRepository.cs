using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IHazardRepository
    {
        IEnumerable<Hazard> hazards { get; }
        void Save(Hazard hazard);
        void Delete(Hazard hazard);
    }
}

