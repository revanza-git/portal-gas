using Admin.Models.Aman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IAmanSourceRepository
    {
        IEnumerable<AmanSource> AmanSources { get; }
        void Save(AmanSource amanSource);
        void Delete(AmanSource source);
    }
}
