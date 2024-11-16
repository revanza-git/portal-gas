using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IFSRUDataService
    {
        IEnumerable<FSRUData> FSRUData { get; }
        void Save(String id, String param, String value);
        IEnumerable<FSRUData> InitiateFSRU(DateTime date);
    }
}
