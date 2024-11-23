using Admin.Data;
using Admin.Interfaces.Services;
using Admin.Models.GCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public class GCGService : IGCGService
    {
        private ApplicationDbContext context;
        public GCGService(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<CocCoi> CocCoi => context.CocCois;

        public CocCoi GetCocCoi(int Year, String UserID)
        {
            CocCoi search = context.CocCois.FirstOrDefault(x => x.Year == Year && x.UserID == UserID);
            if (search == null)
            {
                search = InitCocCoi(Year, UserID);
            }
            return (search);
        }

        public CocCoi InitCocCoi(int Year, String UserID)
        {
            CocCoi c = new CocCoi();
            c.Year = Year;
            c.UserID = UserID;
            c.CoC = false;
            c.CoI = false;
            c.CreatedOn = DateTime.Now;
            c.LastUpdated = DateTime.Now;
            context.CocCois.Add(c);
            context.SaveChanges();
            return (c);
        }

        public void Save(int Year, String UserID, String Type)
        {
            CocCoi search = context.CocCois.FirstOrDefault(x => x.Year == Year && x.UserID == UserID);
            if (search == null)
            {
                search = InitCocCoi(Year, UserID);
            }

            if(Type == "CoI")
            {
                search.CoI = true;
                search.CoISignedTime = DateTime.Now;
                search.LastUpdated = DateTime.Now;
            }
            else if(Type == "CoC")
            {
                search.CoC = true;
                search.CoCSignedTime = DateTime.Now;
                search.LastUpdated = DateTime.Now;
            }
            context.SaveChanges();
        }
    }
}
