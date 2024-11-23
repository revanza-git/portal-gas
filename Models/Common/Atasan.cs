using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Common
{
    public class Atasan
    {
        [Key]
        public string UserName { get; set; }

        public string Name { get; set; }
        public string Department { get; set; }
    }
}

