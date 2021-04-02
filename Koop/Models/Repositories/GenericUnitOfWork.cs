using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Koop.Models.Repositories
{
    public class GenericUnitOfWork : IDisposable, IGenericUnitOfWork
    {
        private bool disposed = false;
        private KoopDbContext _koopDbContext;
        private Dictionary<Type, object> _repositories;
        private Dictionary<Type, object> _repositoriesView;
        private IShopRepository _shopRepository;
        private IMapper _mapper;

        public GenericUnitOfWork(KoopDbContext koopDbContext, IMapper mapper)
        {
            _koopDbContext = koopDbContext;
            _mapper = mapper;
            _repositories = new Dictionary<Type, object>();
            _repositoriesView = new Dictionary<Type, object>();
            _shopRepository = new ShopRepository(_koopDbContext, mapper);
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> newRepository = new GenericRepository<T>(_koopDbContext);
            
            _repositories.Add(typeof(T), newRepository);

            return newRepository;
        }

        public IRepositoryView<T> RepositoryView<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            _koopDbContext.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            var success = await _koopDbContext.SaveChangesAsync();
            return success;
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _koopDbContext.Dispose();
                }
            }

            this.disposed = true;
        }

        public IShopRepository ShopRepository()
        {
            return _shopRepository;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}