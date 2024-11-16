using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IAmanSourceRepository
    {
        IEnumerable<AmanSource> AmanSources { get; }
        void Save(AmanSource amanSource);
        void Delete(AmanSource source);
    }
}
