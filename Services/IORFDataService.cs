using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IORFDataService
    {
        IEnumerable<ORFData> ORFData { get; }
        void Save(String id, String param, String value);
        IEnumerable<ORFData> InitiateORF(DateTime date);
    }
}
