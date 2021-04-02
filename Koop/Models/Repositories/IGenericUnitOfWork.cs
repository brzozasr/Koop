using System.Threading.Tasks;

namespace Koop.Models.Repositories
{
    public interface IGenericUnitOfWork
    {
        KoopDbContext DbContext { get; }
        IRepository<T> Repository<T>() where T : class;
        IRepositoryView<T> RepositoryView<T>() where T : class;
        void SaveChanges();
        Task<int> SaveChangesAsync();
        void Dispose(bool disposing);
        IShopRepository ShopRepository();
    }
}