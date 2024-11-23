using Admin.Models.DCU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IDCUService
    {
        IEnumerable<DCU> DCU { get; }
    }
}
