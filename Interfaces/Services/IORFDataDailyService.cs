using Admin.Models.Gasmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IORFDataDailyService
    {
        IEnumerable<ORFDataDaily> ORFDataDaily { get; }
        void Save(string id, string param, string value);
        List<ORFDataDaily> InitiateORFDaily(DateTime date);
    }
}
