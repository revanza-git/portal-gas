using Admin.Models.Semar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface ISemarRepository
    {
        IEnumerable<Semar> Semars { get; }

        void Save(Semar semar, string mode);
        void Delete(Semar semar);
        Semar Approve(string SemarID);
        string GetNextID();
        void UpdateExpiredNotif(string SemarID);
    }
}
