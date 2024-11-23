using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Vendors
{
    public class Project
    {
        public string ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string VendorID { get; set; }
        public string SponsorPekerjaan { get; set; }
        public string HSSE { get; set; }
        public string PemilikWilayah { get; set; }
        public int status { get; set; }
    }
}
