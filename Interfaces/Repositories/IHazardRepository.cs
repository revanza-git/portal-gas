using Admin.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IHazardRepository
    {
        IEnumerable<Hazard> hazards { get; }
        void Save(Hazard hazard);
        void Delete(Hazard hazard);
    }
}

