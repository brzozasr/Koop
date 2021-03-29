using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Koop.Models.ModelView
{
    [Keyless]
    public class FnListForPacker
    {
        public string ProductName { get; set; }
        public string ProductsInBaskets { get; set; }
    }
}