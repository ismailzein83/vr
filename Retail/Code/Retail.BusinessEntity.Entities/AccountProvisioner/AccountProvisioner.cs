using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class AccountProvisioner
    {
        public abstract void Execute(IAccountProvisioningContext context);
    }
    public interface IAccountProvisioningContext
    {
        AccountProvisionerDefinitionSettings DefinitionSettings { get; }
        long AccountId { get; }
        Guid AccountBEDefinitionId { get; }
    }

   
}
