using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IDCURepository
    {
        IEnumerable<DCU> DCUs{ get; }

        void Save(DCU dcu, String mode);
        void Delete(DCU dcu);
        //DCU Approve(String DCUID);
        String GetNextID();
        //void UpdateExpiredNotif(String DCUID);


        //IEnumerable<DCU> DCUs { get; }
        //void Save(DCU dcu,String mode);
        //IEnumerable<DCUStatus> GetDCUStatuses();
        //String GetNextID();
        //void Delete(DCU dcu);

        
    }
}
