using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class CargoService : ICargoService
    {
        IEnumerable<Cargo> ICargoService.Cargo => throw new NotImplementedException();

        void ICargoService.Save(string id, string date)
        {
            throw new NotImplementedException();
        }
    }
}
