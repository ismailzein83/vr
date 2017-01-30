using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.BusinessProcess.Entities;

namespace Retail.BusinessEntity.Business.Extensions
{
    public class BalanceAlertAccountAction : BaseAccountBalanceAlertVRAction
    {
        public override Guid ConfigId { get { return new Guid("a8e093f4-f9c3-420b-99b7-32eea2c1df78"); } }

        public AccountActionBackendExecutor ActionExecutor { get; set; }

        static AccountActionManager s_accountActionManager = new AccountActionManager();

        public override bool TryConvertToBPInputArgument(IVRActionConvertToBPInputArgumentContext context)
        {
            VRBalanceAlertEventPayload balanceAlertEventPayload = context.EventPayload as VRBalanceAlertEventPayload;
            balanceAlertEventPayload.ThrowIfNull("balanceAlertEventPayload", "");

            Guid accountDefinitionId = base.GetAccountBEDefinitionId(balanceAlertEventPayload);
            long accountId = base.GetAccountId(balanceAlertEventPayload);
            BaseProcessInputArgument bpInputArg;
            if (s_accountActionManager.TryConvertActionToBPArg(accountDefinitionId, accountId, this.ActionExecutor, out bpInputArg))
            {
                context.BPInputArgument = bpInputArg;
                return true;
            }
            else
                return false;
        }

        public override void Execute(IVRActionExecutionContext context)
        {
            VRBalanceAlertEventPayload balanceAlertEventPayload = context.EventPayload as VRBalanceAlertEventPayload;
            balanceAlertEventPayload.ThrowIfNull("balanceAlertEventPayload", "");

            Guid accountDefinitionId = base.GetAccountBEDefinitionId(balanceAlertEventPayload);
            long accountId = base.GetAccountId(balanceAlertEventPayload);
            s_accountActionManager.Execute(accountDefinitionId, accountId, this.ActionExecutor);
        }
    }
}
