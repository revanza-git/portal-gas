using Admin.Models.GCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IPelaporanGratifikasiRepository
    {
        IEnumerable<PelaporanGratifikasi> PelaporanGratifikasis { get; }
        void Save(PelaporanGratifikasi pg);
    }
}
