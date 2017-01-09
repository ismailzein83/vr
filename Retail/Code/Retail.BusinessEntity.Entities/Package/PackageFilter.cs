using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageFilter
    {
        public List<IPackageFilter> Filters { get; set; }

        public List<int> ExcludedPackageIds { get; set; }
    }
}
