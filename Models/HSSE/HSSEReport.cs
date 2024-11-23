using System;

namespace Admin.Models.HSSE
{
    public class HSSEReport
    {
        public string HSSEReportID { get; set; }
        public string Company { get; set; }
        public string Service { get; set; }
        public int PersonOnBoard { get; set; }
        public int SafemanHours { get; set; }
        public int? NumberOfFatalityCase { get; set; }
        public int? NumberOfLTICase { get; set; }
        public int? NumberOfMTC { get; set; }
        public int? NumberOfRWC { get; set; }
        public int? NumberOfFirstAid { get; set; }
        public int NumberOfOilSpill { get; set; }
        public int NumberOfSafetyMeeting { get; set; }
        public string DokumentasiSafetyMeeting { get; set; }
        public int NumberOfToolboxMeeting { get; set; }
        public string DokumentasiToolboxMeeting { get; set; }
        public int NumberOfEmergencyDrill { get; set; }
        public string DokumentasiEmergencyDrill { get; set; }
        public int NumberOfManagementVisit { get; set; }
        public string DokumentasiManagementVisit { get; set; }
        public string ReportedBy { get; set; }
        public DateTime ReportingDate { get; set; }
        public int Status { get; set; }
    }
}
