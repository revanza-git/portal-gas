using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IDCUService
    {
        IEnumerable<DCU> DCU { get; }
    }
}
