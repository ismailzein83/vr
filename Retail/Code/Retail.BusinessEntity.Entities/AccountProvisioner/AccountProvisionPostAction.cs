using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{


    public abstract class AccountProvisionDefinitionPostAction
    {
        public abstract Guid ConfigId { get; }
        public abstract string RuntimeDirective { get; }
    }
    public abstract class AccountProvisionPostAction
    {
        public abstract void ExecutePostAction(IAccountProvisionPostActionContext context);
    }
    public interface IAccountProvisionPostActionContext
    {
        AccountProvisionDefinitionPostAction DefinitionPostAction { get; }
        long AccountId { get; }
        Guid AccountBEDefinitionId { get;  }
    }
    public class AccountProvisionPostActionContext : IAccountProvisionPostActionContext
    {
        public AccountProvisionDefinitionPostAction DefinitionPostAction { get; set; }
        public long AccountId { get; set; }
        public  Guid AccountBEDefinitionId { get; set; }
    }
}
