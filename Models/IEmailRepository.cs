using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IEmailRepository
    {
        void Save(Email email);
        IEnumerable<Email> Emails { get; }
        Email UpdateStatus(int EmailID);
    }
}
