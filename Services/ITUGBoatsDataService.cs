using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface ITUGBoatsDataService
    {
        IEnumerable<TUGBoatsData> TUGBoatsData { get; }
        IEnumerable<Boat> Boats { get; }
        void Save(String id, String param, String value);
        List<TUGBoatsData> InitiateTUGBoatsData(DateTime date);
    }
}
