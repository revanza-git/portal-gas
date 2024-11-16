﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface ISDMRepository
    {
        IEnumerable<Overtime> Overtime { get; }
        void SaveOvertime(Overtime overtime, String mode);
        void DeleteOvertime(Overtime overtime);
        List<Recap> getRecap(int bulan, string username,int tahun);
        void BatchApproval(string[] data);
       
    }
}
