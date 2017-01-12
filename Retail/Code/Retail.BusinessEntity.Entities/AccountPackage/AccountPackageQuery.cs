using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountPackageQuery
    {
        public Guid AccountBEDefinitionId { get; set; }

        public long AssignedToAccountId { get; set; }
    }
}
