using Admin.Models.NOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface INOCService
    {
        IEnumerable<NOC> NOC { get; }
    }
}
