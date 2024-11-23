using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.DCU
{
    public class DCUDownloadHistory
    {
        public int ID { get; set; }
        public string DCUID { get; set; }
        public string UserID { get; set; }
        public DateTime DownloadTime { get; set; }
    }
}
