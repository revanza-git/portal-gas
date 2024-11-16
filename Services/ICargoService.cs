using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface ICargoService
    {
        IEnumerable<Cargo> Cargo { get; }
        void Save(String id, String date);
    }
}
