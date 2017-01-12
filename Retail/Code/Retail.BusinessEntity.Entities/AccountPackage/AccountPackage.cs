using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackage
    {
        public int AccountPackageId { get; set; }

        public long AccountId { get; set; }

        public int PackageId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class AccountPackageToAdd : AccountPackage
    {
        public Guid AccountBEDefinitionId { get; set; }
    }
}
