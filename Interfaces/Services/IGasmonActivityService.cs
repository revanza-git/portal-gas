using Admin.Models.Gasmon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IGasmonActivityService
    {
        IEnumerable<GasmonActivity> GasmonActivity { get; }
        void Save(int source, string time, string remark);
        void Delete(GasmonActivity act);
    }
}
