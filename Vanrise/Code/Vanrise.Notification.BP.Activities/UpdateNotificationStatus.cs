using System.Activities;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;
using Vanrise.BusinessProcess;
using System;

namespace Vanrise.Notification.BP.Activities
{

    public sealed class UpdateNotificationStatus : CodeActivity
    {
        [RequiredArgument]
        public InArgument<long> VRNotificationId { get; set; }

        [RequiredArgument]
        public InArgument<VRNotificationStatus> Status { get; set; }

        public InArgument<VRNotificationBPInstanceType?> VRNotificationBPInstanceType { get; set; }

        public InArgument<IVRActionRollbackEventPayload> RollbackEventPayload { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            VRNotificationBPInstanceType? vrNotificationBPInstanceType = VRNotificationBPInstanceType.Get(context);

            long? executeBPInstanceId = null;
            long? clearBPInstanceId = null;


            if (vrNotificationBPInstanceType.HasValue)
            {
                long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
                switch (vrNotificationBPInstanceType.Value)
                {
                    case Entities.VRNotificationBPInstanceType.Execute: executeBPInstanceId = processInstanceId; break;
                    case Entities.VRNotificationBPInstanceType.Clear: clearBPInstanceId = processInstanceId; break;
                    default: throw new NotSupportedException(string.Format("VRNotificationBPInstanceType '{0}' is not supported", vrNotificationBPInstanceType.Value));
                }
            }

            VRNotificationManager manager = new VRNotificationManager();
            manager.UpdateNotificationStatus(VRNotificationId.Get(context), Status.Get(context), this.RollbackEventPayload.Get(context), executeBPInstanceId, clearBPInstanceId);
        }
    }
}
