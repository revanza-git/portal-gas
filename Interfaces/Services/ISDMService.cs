using Admin.Models.Overtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface ISDMService
    {
        IEnumerable<Overtime> Overtimes { get; }
    }
}
