using Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class EFPelaporanGratifikasiRepository : IPelaporanGratifikasiRepository
    {
        private ApplicationDbContext context;
        public EFPelaporanGratifikasiRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<PelaporanGratifikasi> PelaporanGratifikasis => context.PelaporanGratifikasi;

        public void Save(PelaporanGratifikasi pg)
        {
            if (pg.PelaporanGratifikasiID == 0)
            {
                PelaporanGratifikasi search = context.PelaporanGratifikasi.FirstOrDefault(x => x.Year == pg.Year && x.Month == pg.Month && x.UserID == pg.UserID);
                if (search == null)
                {
                    pg.CreatedOn = DateTime.Now;
                    pg.LastUpdated = DateTime.Now;
                    context.PelaporanGratifikasi.Add(pg);
                }
            }
            else
            {
                PelaporanGratifikasi search = context.PelaporanGratifikasi.FirstOrDefault(x => x.PelaporanGratifikasiID == pg.PelaporanGratifikasiID);
                if (search != null)
                {
                    search.AdaPemberianGratifikasi = pg.AdaPemberianGratifikasi;
                    search.AdaPenerimaanGratifikasi = pg.AdaPenerimaanGratifikasi;
                    search.AdaPermintaanGratifikasi = pg.AdaPermintaanGratifikasi;
                    search.DeskripsiPenerimaanGratifikasi = pg.DeskripsiPenerimaanGratifikasi;
                    search.DeskripsiPemberianGratifikasi = pg.DeskripsiPemberianGratifikasi;
                    search.DeskripsiPermintaanGratifikasi = pg.DeskripsiPermintaanGratifikasi;
                    search.LastUpdated = DateTime.Now;
                }
            }
            context.SaveChanges();
        }
    }
}
