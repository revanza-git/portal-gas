using Admin.Models.DCU;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IDCURepository
    {
        IEnumerable<DCU> DCUs { get; }

        void Save(DCU dcu, string mode);
        void Delete(DCU dcu);
        //DCU Approve(String DCUID);
        string GetNextID();
        //void UpdateExpiredNotif(String DCUID);


        //IEnumerable<DCU> DCUs { get; }
        //void Save(DCU dcu,String mode);
        //IEnumerable<DCUStatus> GetDCUStatuses();
        //String GetNextID();
        //void Delete(DCU dcu);


    }
}
