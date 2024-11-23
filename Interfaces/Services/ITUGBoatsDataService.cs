using Admin.Models.Tugboat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface ITUGBoatsDataService
    {
        IEnumerable<TUGBoatsData> TUGBoatsData { get; }
        IEnumerable<Boat> Boats { get; }
        void Save(string id, string param, string value);
        List<TUGBoatsData> InitiateTUGBoatsData(DateTime date);
    }
}
