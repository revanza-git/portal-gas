using Admin.Models.Gasmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IFSRUDataService
    {
        IEnumerable<FSRUData> FSRUData { get; }
        void Save(string id, string param, string value);
        IEnumerable<FSRUData> InitiateFSRU(DateTime date);
    }
}
