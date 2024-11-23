using Admin.Models.Gasmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IORFDataService
    {
        IEnumerable<ORFData> ORFData { get; }
        void Save(string id, string param, string value);
        IEnumerable<ORFData> InitiateORF(DateTime date);
    }
}
