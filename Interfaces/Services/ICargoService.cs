using Admin.Models.Tugboat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface ICargoService
    {
        IEnumerable<Cargo> Cargo { get; }
        void Save(string id, string date);
    }
}
