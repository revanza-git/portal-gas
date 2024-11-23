using Admin.Data;
using Admin.Interfaces.Services;
using Admin.Models.Gasmon;
using Admin.Models.Tugboat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class GasmonParameterService : IGasmonParameterService
    {
        private ApplicationDbContext context;
        public GasmonParameterService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<GasmonParameter> GasmonParameter => context.GasmonParameter;
        public IEnumerable<Cargo> Cargo => context.Cargos;

        public bool CheckParams(int tahun)
        {
            int count = context.GasmonParameter.Where(x => x.Tahun == tahun).Count();
            if (count > 0)
                return true;
            else
                return false;
        }

        public void InitParams(int tahun)
        {
            if (CheckParams(tahun))
                return;
        
            for (;;)
            {
                try
                {
                    List<GasmonParameter> searchs = context.GasmonParameter.Where(x => x.Tahun == tahun - 1).ToList();
                    foreach (GasmonParameter search in searchs)
                    {
                        search.Tahun = tahun;
                        search.CreatedOn = DateTime.Now;
                        search.LastUpdated = DateTime.Now;
                        context.GasmonParameter.Add(search);
                    }
                    context.SaveChanges();
                    break;
                }
                catch (Exception)
                {
                    tahun--;
                }
                if (tahun <= 2012)
                    break;
            }
        }

        public void Save(String id, Int32 value)
        {
            GasmonParameter search = context.GasmonParameter.FirstOrDefault(x => x.ParameterID == id);
            if(search != null)
            {
                search.Value = value;
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public void UpdateCargo(Int32 id, String code, Int32 tahun, DateTime date)
        {
            Cargo search = context.Cargos.FirstOrDefault(x => x.CargoID == id);
            if (search != null)
            {
                search.Code = code;
                search.Tahun = tahun;
                search.Date = date;
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }

        public void AddCargo(Cargo cargo)
        {
            context.Cargos.Add(cargo);
            context.SaveChanges();
        }

        public void DeleteCargo(Cargo cargo)
        {
            context.Cargos.Remove(cargo);
            context.SaveChanges();
        }
    }
}
