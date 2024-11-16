using Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Services
{
    public interface IGCGService
    { 
        IEnumerable<CocCoi> CocCoi { get; }
        CocCoi InitCocCoi(int Year, String UserID);
        CocCoi GetCocCoi(int Year, String UserID);
        void Save(int Year, String UserID, String Type);
    }
}
