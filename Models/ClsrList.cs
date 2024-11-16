using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
	public class ClsrList
	{
		[Key]
		public int ClsrID { get; set; }

		[Required]
		public string Deskripsi { get; set; }
	}
}
