//using Admin.Data;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Admin.Models
//{
//    public class EFJenisPekerjaanRepository : IJenisPekerjaanRepository
//    {
//        private ApplicationDbContext context;
//        public EFJenisPekerjaanRepository(ApplicationDbContext ctx)
//        {
//            context = ctx;
//        }

//        public IEnumerable<JenisPekerjaan> JenisPekerjaan => context.JenisPekerjaan;

//        public void Save(JenisPekerjaan jenisPekerjaan)
//        {
//            if (jenisPekerjaan.jenis_pekerjaanID == 0)
//            {
//                context.JenisPekerjaan.Add(jenisPekerjaan);
//            }
//            else
//            {
//                JenisPekerjaan search = context.JenisPekerjaan.FirstOrDefault(x => x.jenis_pekerjaanID == jenisPekerjaan.jenis_pekerjaanID);
//                //Location search = context.Locations.FirstOrDefault(x => x.LocationID == location.LocationID);
//                if (search != null)
//                {
//                    search.Deskripsi = jenisPekerjaan.Deskripsi;
//                }
//            }
//            context.SaveChanges();
//        }

//        public void Delete(JenisPekerjaan jenisPekerjaan)
//        {
//            context.JenisPekerjaan.Remove(jenisPekerjaan);
//            context.SaveChanges();
//        }
//    }
//}
