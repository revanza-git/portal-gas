using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IFSRUDataDailyService
    {
        IEnumerable<FSRUDataDaily> FSRUDataDaily { get; }
        void Save(String id, String param, String value);
        FSRUDataDaily InitiateFSRUDaily(DateTime date);
    }
}
