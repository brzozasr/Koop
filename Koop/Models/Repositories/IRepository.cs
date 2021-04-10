using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Koop.models;

namespace Koop.Models.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<List<T>> GetAllAsync();
        T GetDetail(Func<T, bool> predicate);
        void Add(T entity);
        Task AddAsync(T entity);
        void Delete(T entity);
        Task<List<T>> ExecuteSql(string query, params object[] parameters);
    }
}