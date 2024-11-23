using Admin.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Interfaces.Repositories
{
    public interface IEventRepository
    {
        IEnumerable<Event> Events { get; }
    }
}
