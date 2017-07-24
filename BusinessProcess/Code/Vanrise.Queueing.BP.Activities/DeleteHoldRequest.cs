using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.Queueing.BP.Activities
{
    public sealed class DeleteHoldRequest : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            long bpInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            HoldRequestManager holdRequestManager = new HoldRequestManager();
            holdRequestManager.DeleteHoldRequestByBPInstanceId(bpInstanceId);

            //context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Delete Hold Request of BPInstanceId: {0} is done", bpInstanceId);
        }
    }
}
