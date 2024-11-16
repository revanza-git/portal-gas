using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class Email
    {
        public int EmailID { get; set; }
        public String Receiver { get; set; }
        public String Subject { get; set; }
        public String Message { get; set; }
        public int Status { get; set; }
        public DateTime Schedule { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
