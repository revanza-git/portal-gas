using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Aman
{
    public class Reschedule
    {
        public int RescheduleID { set; get; }
        public string AmanID { set; get; }
        public DateTime OldEndDate { set; get; }
        public DateTime NewEndDate { set; get; }
        public string Reason { set; get; }
        public int Status { set; get; }
    }
}
