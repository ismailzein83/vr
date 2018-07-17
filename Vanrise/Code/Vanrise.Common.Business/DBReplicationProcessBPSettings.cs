using System;
using Vanrise.Common;
using Vanrise.Common.BP.Arguments;

namespace Vanrise.Common.Business
{
    public class DBReplicationProcessBPSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            DBReplicationProcessInput inputArg = context.IntanceToRun.InputArgument.CastWithValidate<DBReplicationProcessInput>("context.IntanceToRun.InputArgument");
            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                DBReplicationProcessInput startedBPInstanceInputArg = startedBPInstance.InputArgument as DBReplicationProcessInput;
                if (startedBPInstanceInputArg != null)
                {
                    context.Reason = "Another Database replication process is running";
                    return false;
                }
            }
            return true;
        }

        public override bool StoreLastArgumentState { get { return true; } }
    }
}