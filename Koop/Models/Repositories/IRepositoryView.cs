using System;
using System.Collections.Generic;
using System.Linq;

namespace Koop.Models.Repositories
{
    public interface IRepositoryView<T> where T : class
    {
        IQueryable<T> GetAll();
    }
}