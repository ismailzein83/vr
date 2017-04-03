using Vanrise.BusinessProcess.Business;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Vanrise.Notification.BP.Arguments;
using System;

namespace Vanrise.Notification.Business
{
    public class ExecuteNotificationBPDefinitionExtendedSettings : DefaultBPDefinitionExtendedSettings
    {
        public override void OnBPExecutionCompleted(IBPDefinitionBPExecutionCompletedContext context)
        {
            VRNotificationStatus notificationStatus;
            switch (context.BPInstance.Status)
            {
                case BPInstanceStatus.Completed: notificationStatus = VRNotificationStatus.Executed; break;
                case BPInstanceStatus.Aborted:
                case BPInstanceStatus.Terminated:
                case BPInstanceStatus.Suspended: notificationStatus = VRNotificationStatus.Suspended; break;
                default: throw new NotSupportedException(string.Format("BPInstanceStatus {0} not supported.", context.BPInstance.Status));
            }

            ExecuteNotificationProcessInput inputArgument = context.BPInstance.InputArgument.CastWithValidate<ExecuteNotificationProcessInput>("context.BPInstance.InputArgument", context.BPInstance.ProcessInstanceID);
            new VRNotificationManager().UpdateNotificationStatus(inputArgument.NotificationId, notificationStatus);
        }
    }
}