using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business.AccountProvisioning
{
    public class AccountProvisioningContext : IAccountProvisioningContext
    {
        public AccountProvisionerDefinitionSettings DefinitionSettings { get; set; }
        public long AccountId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
    }
}
