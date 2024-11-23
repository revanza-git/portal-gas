using Admin.Models.Aman;
using Admin.Models.Common;
using Admin.Models.Overtime;
using Admin.Models.Semar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface ICommonRepository
    {
        IEnumerable<Department> GetDepartments();
        IEnumerable<Department> GetAllDepartments();
        IEnumerable<Classification> GetClassifications();
        IEnumerable<Priority> GetPriorities();
        IEnumerable<Hazard> GetHazards();
        IEnumerable<AmanStatus> GetAmanStatuses();
        IEnumerable<Location> GetLocations();
        IEnumerable<JenisPekerjaan> GetJenisPekerjaan();
        IEnumerable<Responsible> GetResponsibles();
        IEnumerable<AmanCorrectionType> GetAmanCorrectionTypes();
        IEnumerable<AmanSource> GetAmanSources();
        IEnumerable<SemarType> GetSemarTypes();
        IEnumerable<SemarProduct> GetSemarProducts();
        IEnumerable<SemarLevel> GetSemarLevels();
        IEnumerable<SemarStatus> GetSemarStatuses();
        IEnumerable<Jabatan> GetJabatan();
        IEnumerable<OvertimeStatus> GetOvertimeStatuses();
        IEnumerable<JamKerjaStatus> GetJamKerjaStatuses();
        IEnumerable<Atasan> GetAtasan();
    }
}
