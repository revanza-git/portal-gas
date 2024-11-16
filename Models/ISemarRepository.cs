using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface ISemarRepository
    {
        IEnumerable<Semar> Semars { get; }
   
        void Save(Semar semar,String mode);
        void Delete(Semar semar);
        Semar Approve(String SemarID);
        String GetNextID();
        void UpdateExpiredNotif(String SemarID);
    }
}
