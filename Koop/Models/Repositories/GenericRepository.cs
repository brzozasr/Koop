using System;
using System.Collections.Generic;
using System.Linq;
using Koop.models;
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

        public IEnumerable<Basket> GetBaskets()
        {
            var baskets = _koopDbContext.Baskets
                .Include(b => b.Coop)
                .Where(b=>b.CoopId != null)
                .Select(b => new Basket()
                {
                    BasketId = b.BasketId,
                    BasketName = b.BasketName,
                    CoopName = $"{b.Coop.FirstName} {b.Coop.LastName}",
                    CoopId = b.CoopId
                });
            
            return baskets;
        }

        public IEnumerable<UserOrdersHistoryView> GetUserOrders(string firstName, string lastName)
        {
            return _koopDbContext.UserOrdersHistoryView.Where(c =>
                c.FirstName.ToLower() == firstName && c.LastName.ToLower() == lastName);
        }

        public Supplier GetSupplier(string abbr)
        {
            var supplier = _koopDbContext.Suppliers
                .Include(s => s.Opro)
                .Include(s=>s.Products)
                .Select(s=> new Supplier()
                    {
                        SupplierId = s.SupplierId,
                        SupplierName = s.SupplierName,
                        SupplierAbbr = s.SupplierAbbr,
                        Description = s.Description,
                        Email = s.Email,
                        Phone = s.Phone,
                        Picture = s.Picture,
                        OrderClosingDate = s.OrderClosingDate,
                        OproId = s.OproId,
                        OproName = $"{s.Opro.FirstName} {s.Opro.LastName}",
                    })
                .SingleOrDefault(s=>s.SupplierAbbr.ToLower() == abbr);
            
            return supplier;
        }
    }
}