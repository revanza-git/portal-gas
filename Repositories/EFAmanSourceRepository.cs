using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Aman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFAmanSourceRepository : IAmanSourceRepository
    {
        private ApplicationDbContext context;
        public EFAmanSourceRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<AmanSource> AmanSources => context.AmanSources;

        public void Save(AmanSource amanSource)
        {
            if (amanSource.AmanSourceID == 0)
            {
                context.AmanSources.Add(amanSource);
            }
            else
            {
                AmanSource search = context.AmanSources.FirstOrDefault(x => x.AmanSourceID == amanSource.AmanSourceID);
                if (search != null)
                {
                    search.Deskripsi = amanSource.Deskripsi;
                }
            }
            context.SaveChanges();
        }

        public void Delete(AmanSource source)
        {
            context.AmanSources.Remove(source);
            context.SaveChanges();
        }
    }
}
