using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Project
    {
        public String ProjectID { get; set;}
        public String ProjectName { get; set; }
        public String VendorID { get; set; }
        public String SponsorPekerjaan { get; set; }
        public String HSSE { get; set; }
        public String PemilikWilayah { get; set; }
        public int status { get; set; }
    }
}
