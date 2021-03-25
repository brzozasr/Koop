using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Koop.Models.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private KoopDbContext _koopDbContext;
        private DbSet<T> _objectSet;

        public GenericRepository(KoopDbContext koopDbContext)
        {
            _koopDbContext = koopDbContext;
            _objectSet = _koopDbContext.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _objectSet;
        }

        public T GetDetail(Func<T, bool> predicate)
        {
            return _objectSet.FirstOrDefault(predicate);
        }

        public void Add(T entity)
        {
            _objectSet.Add(entity);
        }

        public void Delete(T entity)
        {
            _objectSet.Remove(entity);
        }
    }
}