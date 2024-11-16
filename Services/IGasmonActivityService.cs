using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IGasmonActivityService
    {
        IEnumerable<GasmonActivity> GasmonActivity { get; }
        void Save(Int32 source, String time , String remark);
        void Delete(GasmonActivity act);
    }
}
