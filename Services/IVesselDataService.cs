using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IVesselDataService
    {
        IEnumerable<VesselData> VesselData { get; }
        IEnumerable<Vessel> Vessels { get; }
        void Save(String id, String param, String value);
        List<VesselData> InitiateVesselData(DateTime date);
    }
}
