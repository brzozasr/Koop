using Microsoft.EntityFrameworkCore;

namespace Koop.Models.RepositoryModels
{
    [Keyless]
    public class FnListForPacker
    {
        public string ProductName { get; set; }
        public string ProductsInBaskets { get; set; }
    }
}