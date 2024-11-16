using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IORFDataDailyService
    {
        IEnumerable<ORFDataDaily> ORFDataDaily { get; }
        void Save(String id, String param, String value);
        List<ORFDataDaily> InitiateORFDaily(DateTime date);
    }
}
