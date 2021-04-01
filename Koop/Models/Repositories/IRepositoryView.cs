using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Koop.Models.RepositoryModels;
using Microsoft.EntityFrameworkCore;

namespace Koop.Models.Repositories
{
    public interface IRepositoryView<T> where T : class
    {
        IQueryable<T> GetAll();
    }
}