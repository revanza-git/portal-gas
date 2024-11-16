using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IGasmonParameterService
    {
        IEnumerable<GasmonParameter> GasmonParameter { get; }
        IEnumerable<Cargo> Cargo { get; }
        bool CheckParams(int tahun);
        void InitParams(int tahun);
        void Save(String id, Int32 value);
        void UpdateCargo(Int32 id, String code, Int32 tahun, DateTime date);
        void AddCargo(Cargo cargo);
        void DeleteCargo(Cargo cargo);
    }
}
