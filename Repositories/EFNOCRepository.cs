using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Common;
using Admin.Models.NOC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFNOCRepository : INOCRepository
    {
        private readonly ApplicationDbContext context;

        public EFNOCRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<NOC> NOCs => context.NOCs;

        public async Task DeleteAsync(NOC noc)
        {
            context.NOCs.Remove(noc);
            await context.SaveChangesAsync();
        }

        public async Task SaveAsync(NOC noc, string mode)
        {
            if (mode == "add")
            {
                await context.NOCs.AddAsync(noc);
            }
            else
            {
                var search = await context.NOCs.FirstOrDefaultAsync(x => x.NOCID == noc.NOCID);
                if (search != null)
                {
                    search.EntryDate = noc.EntryDate;
                    search.Lokasi = noc.Lokasi;
                    search.DaftarPengamatan = noc.DaftarPengamatan;
                    search.Deskripsi = noc.Deskripsi;
                    search.Tindakan = noc.Tindakan;
                    search.Rekomendasi = noc.Rekomendasi;
                    search.Prioritas = noc.Prioritas;
                    search.Status = noc.Status;
                    search.DueDate = noc.DueDate;
                    search.NamaObserver = noc.NamaObserver;
                    search.DivisiObserver = noc.DivisiObserver;
                    search.Clsr = noc.Clsr;
                    if (noc.Photo != null)
                    {
                        search.Photo = noc.Photo;
                    }
                    search.Status = noc.Status;
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ObservationList>> GetObservationListsAsync()
        {
            return await context.ObservationLists.ToListAsync();
        }

        public async Task<IEnumerable<ClsrList>> GetClsrListsAsync()
        {
            return await context.ClsrLists.ToListAsync();
        }

        public IEnumerable<NOCStatus> GetNOCStatuses()
        {
            return new List<NOCStatus> {
                new NOCStatus () {  NOCStatusID = 1, Deskripsi = "Open"},
                new NOCStatus () {  NOCStatusID = 2, Deskripsi = "Closed"}
            };
        }

        public async Task<string> GetNextIDAsync()
        {
            string ID;

            try
            {
                var noc = await context.NOCs.OrderByDescending(x => x.NOCID).FirstAsync();
                string LastID = noc.NOCID.Substring(3);
                int NextID = int.Parse(LastID) + 1;
                ID = "NOC" + NextID.ToString().PadLeft(4, '0');
            }
            catch (Exception)
            {
                ID = "NOC0001";
            }
            return ID;
        }
    }
}
