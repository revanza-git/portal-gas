using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class News
    {
        public int NewsID { get; set; }
        public String Subject { get; set; }
        public String Content { get; set; }
        public String Department { get; set; }
        public String Author { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime PublishingDate { get; set; }
        public int Counter { get; set; }
        public int Status { get; set; }
    }
}
