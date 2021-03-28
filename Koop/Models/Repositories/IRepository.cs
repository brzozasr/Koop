using System;
using System.Collections.Generic;
using System.Linq;
using Koop.models;

namespace Koop.Models.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetDetail(Func<T, bool> predicate);
        void Add(T entity);
        void Delete(T entity);

        IEnumerable<Basket> GetBaskets();
        IEnumerable<UserOrdersHistoryView> GetUserOrders(string firstName, string lastName);
        Supplier GetSupplier(string abbr);
    }
}