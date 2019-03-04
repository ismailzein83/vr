using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.Business;
using Retail.BusinessEntity.BP.Arguments;

namespace Retail.BusinessEntity.Business
{
    public class AccountBulkActionsBPDefinitionSettings : DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            AccountBulkActionProcessInput accountBulkActionProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<AccountBulkActionProcessInput>("context.IntanceToRun.InputArgument");
            if (accountBulkActionProcessInput.AccountBulkActions == null || accountBulkActionProcessInput.AccountBulkActions.Count == 0)
            {
                context.Reason = String.Format("There are no actions to execute.");
                return false;
            }
            return true;
        }
        public override bool DoesUserHaveStartSpecificInstanceAccess(IBPDefinitionDoesUserHaveStartSpecificInstanceAccessContext context)
        {
            AccountBulkActionProcessInput accountBulkActionProcessInput = context.InputArg.CastWithValidate<AccountBulkActionProcessInput>("context.IntanceToRun.InputArgument");
            if (accountBulkActionProcessInput != null)
            {
                return new AccountBEDefinitionManager().DoesUserHaveStartActionAccess(accountBulkActionProcessInput.AccountBulkActions, accountBulkActionProcessInput.AccountBEDefinitionId, context.DefinitionContext.UserId);
            }
            return false;
        }
        public override bool DoesUserHaveScheduleSpecificTaskAccess(IBPDefinitionDoesUserHaveScheduleSpecificTaskAccessContext context)
        {
            AccountBulkActionProcessInput accountBulkActionProcessInput = context.InputArg.CastWithValidate<AccountBulkActionProcessInput>("context.IntanceToRun.InputArgument");
            if (accountBulkActionProcessInput != null)
            {
                return new AccountBEDefinitionManager().DoesUserHaveRunActionAccess(accountBulkActionProcessInput.AccountBulkActions, accountBulkActionProcessInput.AccountBEDefinitionId, context.DefinitionContext.UserId);
            }
            return false;
        }
        public override bool DoesUserHaveViewAccess(IBPDefinitionDoesUserHaveViewAccessContext context)
        {
            return new AccountBEDefinitionManager().DoesUserHaveActionsAccess(context.UserId);
        }
        public override bool DoesUserHaveStartAccess(IBPDefinitionDoesUserHaveStartAccessContext context)
        {
            return new AccountBEDefinitionManager().DoesUserHaveActionsAccess(context.UserId);
        }
        public override bool DoesUserHaveScheduleTaskAccess(IBPDefinitionDoesUserHaveScheduleTaskContext context)
        {
            return new AccountBEDefinitionManager().DoesUserHaveActionsAccess(context.UserId);
        }
    }
}
