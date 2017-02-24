using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountActionDefinition
    {
        public Guid AccountActionDefinitionId { get; set; }

        public string Name { get; set; }

        public bool VisibleInActionMenu { get; set; }

        public bool VisibleInBalanceAlertRule { get; set; }

        public AccountCondition AvailabilityCondition { get; set; }

        public AccountActionDefinitionSettings ActionDefinitionSettings { get; set; }
    }

    public abstract class AccountActionDefinitionSettings
    {
        public abstract Guid ConfigId { get; }

        public virtual string ClientActionName
        {
            get
            {
                return null;
            }
        }

        public virtual string BackendExecutorSettingEditor
        {
            get
            {
                return null;
            }
        }

        public virtual bool DoesUserHaveAccess(IAccountActionDefinitionCheckAccessContext context)
        {
            return true;
        }
    }
    public interface IAccountActionDefinitionCheckAccessContext
    {
        Guid AccountBEDefinitionId { get; }
        int UserId { get; }
    }

   
    public abstract class AccountActionBackendExecutor
    {
        public Guid ActionDefinitionId { get; set; }

        public virtual bool TryConvertToBPArg(IAccountActionBackendExecutorConvertToBPArgContext context)
        {
            return false;
        }

        public virtual void Execute(IAccountActionBackendExecutorExecuteContext context)
        {
            throw new NotImplementedException();
        }
    }

    public interface IAccountActionBackendExecutorConvertToBPArgContext
    {
        Guid AccountBEDefinitionId { get; }

        long AccountId { get; }

        Account Account { get; }

        BaseProcessInputArgument BPInputArgument { set; }
    }

    public interface IAccountActionBackendExecutorExecuteContext
    {
        Guid AccountBEDefinitionId { get; }

        long AccountId { get; }

        Account Account { get; }
    }
}
