using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.User
{
    public class Email
    {
        public int EmailID { get; set; }
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public DateTime Schedule { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
