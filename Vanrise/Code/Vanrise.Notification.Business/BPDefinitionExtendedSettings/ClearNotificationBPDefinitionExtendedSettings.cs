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
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            ClearNotificationInput executeNotificationProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<ClearNotificationInput>("context.IntanceToRun.InputArgument", context.IntanceToRun.ProcessInstanceID);
            var startedInstances = context.GetStartedBPInstances();
            if (startedInstances != null)
            {
                foreach (var bpInstance in startedInstances)
                {
                    ClearNotificationInput inputArgAsClearNofication = bpInstance.InputArgument as ClearNotificationInput;
                    if (inputArgAsClearNofication != null)
                    {
                        if (inputArgAsClearNofication.NotificationTypeId == executeNotificationProcessInput.NotificationTypeId && inputArgAsClearNofication.EntityId == executeNotificationProcessInput.EntityId)
                        {
                            context.Reason = "Another notification is being cleared for the same Entity";
                            return false;
                        }
                    }
                }
            }
            return base.CanRunBPInstance(context);
        }
    }
}
