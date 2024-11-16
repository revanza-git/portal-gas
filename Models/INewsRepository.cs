﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Admin.Models
{
    public interface INewsRepository
    {
        IEnumerable<News> News { get; }
        void Save(News news);
        void Submit(News news);
        void Publish(News news);
        void Delete(News news);
    }
}
