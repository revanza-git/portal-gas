using Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class EFHazardRepository : IHazardRepository
    {
        private ApplicationDbContext context;
        public EFHazardRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Hazard> hazards => context.Hazards;

        public void Save(Hazard hazard)
        {
            if (hazard.HazardID == 0)
            {
                context.Hazards.Add(hazard);
            }
            else
            {
                Hazard search = context.Hazards.FirstOrDefault(x => x.HazardID == hazard.HazardID);
                if (search != null)
                {
                    search.HazardID = hazard.HazardID;
                }
            }
            context.SaveChanges();
        }

        public void Delete(Hazard hazard)
        {
            context.Hazards.Remove(hazard);
            context.SaveChanges();
        }
    }
}
