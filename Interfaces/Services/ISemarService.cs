using Admin.Models.Semar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface ISemarService
    {
        IEnumerable<Semar> Semars { get; }
    }
}
