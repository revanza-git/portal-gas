using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Semar
{
    public class SemarDownloadHistory
    {
        public int ID { get; set; }
        public string AmanID { get; set; }
        public string UserID { get; set; }
        public DateTime DownloadTime { get; set; }
    }
}
