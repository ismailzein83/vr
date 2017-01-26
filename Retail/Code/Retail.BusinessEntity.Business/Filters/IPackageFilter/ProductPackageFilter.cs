using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public class ProductPackageFilter : IPackageFilter
    {
        public int ProductId { get; set; }
        public bool IsMatched(IPackageFilterContext context)
        {
            IEnumerable<int> packageIds = new ProductManager().GetProductPackageIds(ProductId);

            if (packageIds == null || !packageIds.Contains(context.Package.PackageId))
                return false;

            return true;
        }
    }
}
