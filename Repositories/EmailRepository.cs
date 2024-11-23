using Admin.Data;
using Admin.Interfaces.Repositories;
using Admin.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private ApplicationDbContext context;
        public EmailRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public void Save(Email email)
        {
            context.Emails.Add(email);
            context.SaveChanges();
        }

        public IEnumerable<Email> Emails => context.Emails.Where(x => x.Status == 0);

        public Email UpdateStatus(int EmailID)
        {
            Email search = context.Emails.FirstOrDefault(x => x.EmailID == EmailID);
            if (search != null)
            {
                search.Status = 1;
                context.SaveChanges();
            }
            return search;
        }
    }
}
