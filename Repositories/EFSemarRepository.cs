using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Semar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFSemarRepository : ISemarRepository
    {
        private ApplicationDbContext context;
        public EFSemarRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Semar> Semars => context.Semars;

        public void Delete(Semar semar)
        {
            context.Semars.Remove(semar);
            context.SaveChanges();
        }

        public void Save(Semar semar, string mode)
        {
            if (mode == "add")
            {
                semar.Classification = 2;
                context.Semars.Add(semar);
            }
            else
            {
                Semar search = context.Semars.FirstOrDefault(a => a.SemarID == semar.SemarID);
                if (search != null)
                {
                    search.Type = semar.Type;
                    search.Status = semar.Status;
                    search.SemarLevel = semar.SemarLevel;
                    search.NoDocument = semar.NoDocument;
                    search.PublishDate = semar.PublishDate;
                    search.ExpiredDate = semar.ExpiredDate;
                    search.Revision = semar.Revision;
                    search.Owner = semar.Owner;
                    search.Title = semar.Title;
                    if (semar.FileName != null)
                    {
                        search.FileName = semar.FileName;
                        search.ContentType = semar.ContentType;
                    }
                    search.Classification = 2;
                    search.Description = semar.Description;
                }
            }
            context.SaveChanges();
        }

        public Semar Approve(string SemarID)
        {
            Semar search = context.Semars.FirstOrDefault(x => string.Compare(x.SemarID, SemarID) == 0);
            if (search != null)
            {
                search.Status = 2;
                context.SaveChanges();
            }
            return search;
        }

        public string GetNextID()
        {
            string ID;
            try
            {
                Semar semar = context.Semars.OrderByDescending(x => x.SemarID).First();
                string LastID = semar.SemarID.Substring(5);
                int NextID = int.Parse(LastID) + 1;
                ID = "SEMAR" + NextID.ToString().PadLeft(4, '0');
            }
            catch (Exception)
            {
                ID = "SEMAR0001";
            }
            return ID;
        }

        public void UpdateExpiredNotif(string SemarID)
        {
            Semar search = context.Semars.FirstOrDefault(x => string.Compare(x.SemarID, SemarID) == 0);
            if (search != null)
            {
                search.ExpiredNotification += 1;
                context.SaveChanges();
            }
        }
    }
}
