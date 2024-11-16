using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class ProjectTask
    { 
        public String ProjectTaskID { get; set; }
        public String TraID { get; set; }
        public String SequenceOfBasicJobSteps { get; set; }
        public String Hazard { get; set; }
        public String Consequence { get; set; }
        public String InitialRisk { get; set; }
        public String RecommendedAction { get; set; }
        public String RoleResponsibility { get; set; }
        public String ResidualRisk { get; set; }
        public String ALARP { get; set; }
        public String AC { get; set; }
    }
}
