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
        public override bool CanRunBPInstance(IBPDefinitionCanRunBPInstanceContext context)
        {
            var startedInstances = context.GetStartedBPInstances();
            if(startedInstances != null && startedInstances.Count > 0)
            {
                context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
                ExecuteNotificationProcessInput currentInstanceProcessInput = context.IntanceToRun.InputArgument.CastWithValidate<ExecuteNotificationProcessInput>("context.IntanceToRun.InputArgument", context.IntanceToRun.ProcessInstanceID);
            
                foreach(var bpInstance in startedInstances)
                {
                    INotificationProcessArgument otherInstanceProcessArg = bpInstance.InputArgument as INotificationProcessArgument;
                    if (otherInstanceProcessArg != null)
                    {
                        if(IsNotificationProcessArgRelatedToSameEntity(currentInstanceProcessInput, otherInstanceProcessArg))
                        {
                            context.Reason = "Another notification is running for the same Entity";
                            return false;
                        }
                    }
                }
            }
            return base.CanRunBPInstance(context);
        }
        public override void OnBPExecutionCompleted(IBPDefinitionBPExecutionCompletedContext context)
        {
            if (context.BPInstance.Status == BPInstanceStatus.Completed) // It will be updated from the Workflow
                return;

            VRNotificationStatus notificationStatus;
            switch (context.BPInstance.Status)
            {
                case BPInstanceStatus.Aborted:
                case BPInstanceStatus.Terminated:
                case BPInstanceStatus.Suspended: notificationStatus = VRNotificationStatus.ExecutionError; break;
                default: throw new NotSupportedException(string.Format("BPInstanceStatus {0} not supported.", context.BPInstance.Status));
            }

            ExecuteNotificationProcessInput inputArgument = context.BPInstance.InputArgument.CastWithValidate<ExecuteNotificationProcessInput>("context.BPInstance.InputArgument", context.BPInstance.ProcessInstanceID);
            new VRNotificationManager().UpdateNotificationStatus(inputArgument.NotificationId, notificationStatus);
        }

        internal static bool IsNotificationProcessArgRelatedToSameEntity(INotificationProcessArgument arg1, INotificationProcessArgument arg2)
        {
            if (arg1.NotificationTypeId != arg2.NotificationTypeId)
                return false;
            if (!String.IsNullOrEmpty(arg1.EntityId))
            {
                if (arg1.EntityId != arg2.EntityId)
                    return false;
            }
            else
            {
                if (arg1.EventKey != arg2.EventKey)
                    return false;
            }
            return true;
        }
    }
}