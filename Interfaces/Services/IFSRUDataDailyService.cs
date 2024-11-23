using Admin.Models.Gasmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IFSRUDataDailyService
    {
        IEnumerable<FSRUDataDaily> FSRUDataDaily { get; }
        void Save(string id, string param, string value);
        FSRUDataDaily InitiateFSRUDaily(DateTime date);
    }
}
