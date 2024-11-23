using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.News
{
    public class News
    {
        public int NewsID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Department { get; set; }
        public string Author { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime PublishingDate { get; set; }
        public int Counter { get; set; }
        public int Status { get; set; }
    }
}
