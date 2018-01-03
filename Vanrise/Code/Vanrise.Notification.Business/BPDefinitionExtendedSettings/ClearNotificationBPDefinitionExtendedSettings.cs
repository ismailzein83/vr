using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Business;
using Vanrise.Common;
using Vanrise.Notification.BP.Arguments;

namespace Vanrise.Notification.Business
{
    class ClearNotificationBPDefinitionExtendedSettings : DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            var startedInstances = context.GetStartedBPInstances();
            if (startedInstances != null && startedInstances.Count > 0)
            {
                context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
                ClearNotificationInput currentInstanceProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<ClearNotificationInput>("context.IntanceToRun.InputArgument", context.IntanceToRun.ProcessInstanceID);

                foreach (var bpInstance in startedInstances)
                {
                    INotificationProcessArgument otherInstanceProcessArg = bpInstance.InputArgument as INotificationProcessArgument;
                    if (otherInstanceProcessArg != null)
                    {
                        if (ExecuteNotificationBPDefinitionExtendedSettings.IsNotificationProcessArgRelatedToSameEntity(currentInstanceProcessInput, otherInstanceProcessArg))
                        {
                            context.Reason = "Another notification is running for the same Entity";
                            return false;
                        }
                    }
                }
            }
            return base.CanRunBPInstance(context);
        }
    }
}
