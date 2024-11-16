using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IAmanCorrectionTypeRepository
    {
        IEnumerable<AmanCorrectionType> AmanCorrectionTypes { get; }
        void Save(AmanCorrectionType amanCorrectionType);
        void Delete(AmanCorrectionType correctionType);
    }
}
