using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface IEventRepository
    {
        IEnumerable<Event> Events { get; }
    }
}
