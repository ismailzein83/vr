using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.BP.Arguments;
using Vanrise.Common;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceUpdateProcessBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            AccountBalanceUpdateProcessInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<AccountBalanceUpdateProcessInput>("context.IntanceToRun.InputArgument");
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                AccountBalanceUpdateProcessInput startedBPInstanceInputArg = startedBPInstance.InputArgument as AccountBalanceUpdateProcessInput;
                if (startedBPInstanceInputArg != null)
                {
                    if (startedBPInstanceInputArg.AccountTypeId == inputArg.AccountTypeId)
                    {
                        context.Reason = "Another process is running for the same account type";
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool ShouldCreateScheduledInstance(BusinessProcess.Entities.IBPDefinitionShouldCreateScheduledInstanceContext context)
        {
            AccountBalanceUpdateProcessInput inputArg = context.BaseProcessInputArgument.CastWithValidate<AccountBalanceUpdateProcessInput>("context.BaseProcessInputArgument");

            UsageBalanceManager usageBalanceManager = new UsageBalanceManager();
            bool hasUsageBalanceData = usageBalanceManager.HasUsageBalanceData(inputArg.AccountTypeId);

            if (hasUsageBalanceData)
                return true;

            BillingTransactionManager billingTransactionManager = new BillingTransactionManager();
            bool hasBillingTransactionData = billingTransactionManager.HasBillingTransactionData(inputArg.AccountTypeId);

            if (hasBillingTransactionData)
                return true;

            return false;
        }
    }
}