using Admin.Models.GCG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Services
{
    public interface IGCGService
    {
        IEnumerable<CocCoi> CocCoi { get; }
        CocCoi InitCocCoi(int Year, string UserID);
        CocCoi GetCocCoi(int Year, string UserID);
        void Save(int Year, string UserID, string Type);
    }
}
