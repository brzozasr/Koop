using System.Collections.Generic;
using System.Linq;
using Koop.Models.RepositoryModels;

namespace Koop.Extensions
{
    public static class UtilExt
    {
        public static int AreEquals(this List<SupplierReportItem> items, SupplierReportItem item)
        {
            if (items.Any())
            {
                var j = 0;
                foreach (var i in items)
                {
                    if (i.Equals(item))
                    {
                        return j;
                    }

                    j++;
                }
            }

            return -1;
        }
    }
}