using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models.Vendors
{
    public class ProjectTask
    {
        public string ProjectTaskID { get; set; }
        public string TraID { get; set; }
        public string SequenceOfBasicJobSteps { get; set; }
        public string Hazard { get; set; }
        public string Consequence { get; set; }
        public string InitialRisk { get; set; }
        public string RecommendedAction { get; set; }
        public string RoleResponsibility { get; set; }
        public string ResidualRisk { get; set; }
        public string ALARP { get; set; }
        public string AC { get; set; }
    }
}
