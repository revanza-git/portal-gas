using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class AccidentTriangle
    {
        public int UnsafeAndActCondition { get; set; }
        public int NearMiss { get; set; }
        public int FirstAid { get; set; }
        public int MedicalTreatmentCase { get; set; }
        public int RestrictedWorkCase { get; set; }
        public int LostTimeInjury { get; set; }
        public int Fatality { get; set; }
    }
}
