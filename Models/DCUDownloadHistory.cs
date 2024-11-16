using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class DCUDownloadHistory
    {
        public int ID { get; set; }
        public String DCUID { get; set; }
        public String UserID { get; set; }
        public DateTime DownloadTime { get; set; }
    }
}
