using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Reschedule
    {
        public int RescheduleID { set; get; }
        public String AmanID { set; get; }
        public DateTime OldEndDate { set; get; }
        public DateTime NewEndDate { set; get; }
        public String Reason { set; get; }
        public int Status { set; get; }
    }
}
