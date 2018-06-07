using System;
using System.Activities;
using Vanrise.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareZoneCodeGroupForApplyInput
    {
        public BaseQueue<ZoneCodeGroupBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareZoneCodeGroupForApply : DependentAsyncActivity<PrepareZoneCodeGroupForApplyInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<ZoneCodeGroupBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareZoneCodeGroupForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRPZoneCodeGroupDataManager rpZoneCodeGroupDataManager = RoutingDataManagerFactory.GetDataManager<IRPZoneCodeGroupDataManager>();
            PrepareDataForDBApply(previousActivityStatus, handle, rpZoneCodeGroupDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ZoneCodeGroupBatch => ZoneCodeGroupBatch.ZoneCodeGroups);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Zone Code Group For Apply is done", null);
        }

        protected override PrepareZoneCodeGroupForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareZoneCodeGroupForApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<Object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}