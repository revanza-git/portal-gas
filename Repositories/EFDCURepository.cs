using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.DCU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFDCURepository : IDCURepository
    {
        private ApplicationDbContext context;
        public EFDCURepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<DCU> DCUs => context.DCUs;

        public void Delete(DCU dcu)
        {
            context.DCUs.Remove(dcu);
            context.SaveChanges();
        }

        public void Save(DCU dcu, string mode)
        {
            if (mode == "add")
            {
                //semar.SemarID = GetNextID();
                //semar.Status = semar.Type == 1 ? 1 : 2;
                context.DCUs.Add(dcu);
            }
            else
            {
                DCU search = context.DCUs.FirstOrDefault(a => a.DCUID == dcu.DCUID);
                if (search != null)
                {
                    search.Date = DateTime.Now;
                    search.Nama = dcu.Nama;
                    search.Suhu = dcu.Suhu;
                    search.Diastole = dcu.Diastole;
                    search.Nadi = dcu.Nadi;
                    search.Sistole = dcu.Sistole;
                    search.Keluhan = dcu.Keluhan;
                    search.JenisPekerjaan = dcu.JenisPekerjaan;
                    if (dcu.Foto != null)
                    {
                        search.Foto = dcu.Foto;
                        search.ContentType = dcu.ContentType;
                    }
                }
            }
            context.SaveChanges();
        }

        //public DCU Approve(String DCUID)
        //{
        //    DCU search = context.DCUs.FirstOrDefault(x => String.Compare(x.DCUID, DCUID) == 0);
        //    if (search != null)
        //    {
        //        //search.Status = 2;
        //        context.SaveChanges();
        //    }
        //    return search;
        //}

        public string GetNextID()
        {
            string ID;
            try
            {
                DCU dcu = context.DCUs.OrderByDescending(x => x.DCUID).First();
                string LastID = dcu.DCUID.Substring(3);
                int NextID = int.Parse(LastID) + 1;
                ID = "DCU" + NextID.ToString().PadLeft(5, '0');
            }
            catch (Exception)
            {
                ID = "DCU00001";
            }
            return ID;


        }

        //public void UpdateExpiredNotif(String DCUID)
        //{
        //    DCU search = context.DCUs.FirstOrDefault(x => String.Compare(x.DCUID, DCUID) == 0);
        //    if (search != null)
        //    {
        //        search.ExpiredNotification += 1;
        //        context.SaveChanges();
        //    }
        //}
    }
}
