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
    }
}
