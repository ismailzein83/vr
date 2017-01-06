using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class PackageFilter
    {
        public int? AssignedToAccountId { get; set; }

        public List<int> ExcludedPackageIds { get; set; }

        public List<IPackageFilter> Filters { get; set; }
    }
}
