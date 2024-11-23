using Admin.Models.Gasmon;
using Admin.Models.Tugboat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IGasmonParameterService
    {
        IEnumerable<GasmonParameter> GasmonParameter { get; }
        IEnumerable<Cargo> Cargo { get; }
        bool CheckParams(int tahun);
        void InitParams(int tahun);
        void Save(string id, int value);
        void UpdateCargo(int id, string code, int tahun, DateTime date);
        void AddCargo(Cargo cargo);
        void DeleteCargo(Cargo cargo);
    }
}
