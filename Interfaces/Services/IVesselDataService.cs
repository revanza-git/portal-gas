using Admin.Models.Gasmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IVesselDataService
    {
        IEnumerable<VesselData> VesselData { get; }
        IEnumerable<Vessel> Vessels { get; }
        void Save(string id, string param, string value);
        List<VesselData> InitiateVesselData(DateTime date);
    }
}
