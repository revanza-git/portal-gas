using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.Aman;
using Admin.Models.Common;
using Admin.Models.Overtime;
using Admin.Models.Semar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EFCommonRepository : ICommonRepository
    {
        private ApplicationDbContext context;
        public EFCommonRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IEnumerable<Department> GetDepartments()
        {
            return context.Departments?.Where(d => d.IsDepartment == "Y").ToList() ?? new List<Department>();
        }

        public IEnumerable<Department> GetAllDepartments()
        {
            return context.Departments?.ToList() ?? new List<Department>();
        }

        public IEnumerable<Classification> GetClassifications()
        {
            return new List<Classification>
            {
                new Classification() { ClassificationID = 1, Deskripsi = "Rahasia"},
                new Classification() { ClassificationID = 2, Deskripsi = "Tidak Rahasia"}
            };
        }

        public IEnumerable<Priority> GetPriorities()
        {
            return new List<Priority>
            {
                new Priority() { PriorityID = 1, Deskripsi = "1.High"},
                new Priority() { PriorityID = 2, Deskripsi = "2.Medium"},
                new Priority() { PriorityID = 3, Deskripsi = "3.Low"}
            };
        }

        public IEnumerable<AmanStatus> GetAmanStatuses()
        {
            return new List<AmanStatus> {
                new AmanStatus () {  AmanStatusID = 1, Deskripsi = "Pending"},
                new AmanStatus () {  AmanStatusID = 2, Deskripsi = "Active"},
                new AmanStatus () {  AmanStatusID = 3, Deskripsi = "Closed"}
            };
        }

        public IEnumerable<Location> GetLocations()
        {
            return context.Locations?.ToList() ?? new List<Location>();
        }

        public IEnumerable<JenisPekerjaan> GetJenisPekerjaan()
        {
            return context.JenisPekerjaan?.ToList() ?? new List<JenisPekerjaan>();
        }

        public IEnumerable<Responsible> GetResponsibles()
        {
            return context.Responsibles?.ToList() ?? new List<Responsible>();
        }

        public IEnumerable<AmanCorrectionType> GetAmanCorrectionTypes()
        {
            return context.AmanCorrectionTypes?.ToList() ?? new List<AmanCorrectionType>();
        }

        public IEnumerable<AmanSource> GetAmanSources()
        {
            return context.AmanSources?.ToList() ?? new List<AmanSource>();
        }

        public IEnumerable<SemarType> GetSemarTypes()
        {
            return context.SemarTypes?.ToList() ?? new List<SemarType>();
        }

        public IEnumerable<SemarProduct> GetSemarProducts()
        {
            return new List<SemarProduct> {
                new SemarProduct () {  SemarProductID = 1, Deskripsi = "STK"},
                new SemarProduct () {  SemarProductID = 2, Deskripsi = "SK"},
                new SemarProduct () {  SemarProductID = 3, Deskripsi = "SPRINT"}
            };
        }

        public IEnumerable<SemarLevel> GetSemarLevels()
        {
            return context.SemarLevels?.ToList() ?? new List<SemarLevel>();
        }

        public IEnumerable<SemarStatus> GetSemarStatuses()
        {
            return new List<SemarStatus> {
                new SemarStatus () {  SemarStatusID = 1, Deskripsi = "Aktif"},
                new SemarStatus () {  SemarStatusID = 2, Deskripsi = "Tidak Aktif"}
            };
        }

        public IEnumerable<Hazard> GetHazards()
        {
            return context.Hazards?.ToList() ?? new List<Hazard>();
        }

        public IEnumerable<Jabatan> GetJabatan()
        {
            return context.Jabatan?.ToList() ?? new List<Jabatan>();
        }

        public IEnumerable<OvertimeStatus> GetOvertimeStatuses()
        {
            return new List<OvertimeStatus> {
                new OvertimeStatus () {  OvertimeStatusID = 1, Deskripsi = "Pending"},
                new OvertimeStatus () {  OvertimeStatusID = 2, Deskripsi = "Approved"},
                new OvertimeStatus () {  OvertimeStatusID = 3, Deskripsi = "Reject"}
            };
        }

        public IEnumerable<JamKerjaStatus> GetJamKerjaStatuses()
        {
            return new List<JamKerjaStatus> {
                new JamKerjaStatus () {  JamKerjaStatusID = "WFO", Deskripsi = "WFO"},
                new JamKerjaStatus () {  JamKerjaStatusID = "WFH", Deskripsi = "WFH"},
                new JamKerjaStatus () {  JamKerjaStatusID = "LIBUR", Deskripsi = "LIBUR"}
            };
        }


        public IEnumerable<Atasan> GetAtasan()
        {
            return context.Atasan?.ToList() ?? new List<Atasan>();
        }
    }
}
