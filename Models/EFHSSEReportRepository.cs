using Admin.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public class EFHSSEReportRepository : IHSSEReportRepository
    {
        private ApplicationDbContext context;
        public EFHSSEReportRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }
        public IEnumerable<HSSEReport> HSSEReports => context.HSSEReports;
        public void Delete(HSSEReport report)
        {
            context.HSSEReports.Remove(report);
            context.SaveChanges();
        }
        public void Save(HSSEReport report, String mode)
        {
            if (mode == "add")
            {
                report.ReportingDate = DateTime.Now;
                context.HSSEReports.Add(report);
            }
            else
            {
                HSSEReport search = context.HSSEReports.FirstOrDefault(x => x.HSSEReportID == report.HSSEReportID);
                if (search != null)
                {
                    search.Service = report.Service;
                    search.PersonOnBoard = report.PersonOnBoard;
                    search.SafemanHours = report.SafemanHours;
                    search.NumberOfFatalityCase = report.NumberOfFatalityCase;
                    search.NumberOfLTICase = report.NumberOfLTICase;
                    search.NumberOfMTC = report.NumberOfMTC;
                    search.NumberOfFirstAid = report.NumberOfFirstAid;
                    search.NumberOfOilSpill = report.NumberOfOilSpill;
                    search.NumberOfSafetyMeeting = report.NumberOfSafetyMeeting;
                    search.NumberOfToolboxMeeting = report.NumberOfToolboxMeeting;
                    search.NumberOfEmergencyDrill = report.NumberOfEmergencyDrill;
                    search.NumberOfManagementVisit = report.NumberOfManagementVisit;
                    search.ReportingDate = DateTime.Now;
                    search.ReportedBy = report.ReportedBy;
                    
                    if (report.DokumentasiSafetyMeeting != null)
                    {
                        search.DokumentasiSafetyMeeting = report.DokumentasiSafetyMeeting; 
                    }

                    if(report.DokumentasiToolboxMeeting != null)
                    {
                        search.DokumentasiToolboxMeeting = report.DokumentasiToolboxMeeting;
                    }

                    if(report.DokumentasiEmergencyDrill != null)
                    {
                        search.DokumentasiEmergencyDrill = report.DokumentasiEmergencyDrill;
                    }

                    if(report.DokumentasiManagementVisit != null)
                    {
                        search.DokumentasiManagementVisit = report.DokumentasiManagementVisit;
                    }                    
                }
            }
            context.SaveChanges();
        }

        public String GetNextID()
        {
            String ID;

            try
            {
                HSSEReport report = context.HSSEReports.Where(x => x.HSSEReportID.Contains("HSSENR")).OrderByDescending(x => x.HSSEReportID).First();
                String LastID = report.HSSEReportID.Substring(6);
                int NextID = Int32.Parse(LastID) + 1;
                ID = "HSSENR" + NextID.ToString().PadLeft(4, '0');
            }
            catch (Exception)
            {
                ID = "HSSENR0001";
            }
            return (ID);
        }
    }
}
