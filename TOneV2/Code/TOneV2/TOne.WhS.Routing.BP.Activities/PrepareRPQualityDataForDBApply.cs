using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareRPQualityDataForDBApplyInput
    {
        public int RoutingDatabaseId { get; set; }
        public BaseQueue<RouteRuleQualityConfigurationDataBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareRPQualityDataForDBApply : DependentAsyncActivity<PrepareRPQualityDataForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RouteRuleQualityConfigurationDataBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PrepareRPQualityDataForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRPQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPQualityConfigurationDataManager>();
            dataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, (batch) =>
            {
                return batch.RoutingQualityConfigurationDataList.Select(itm => itm as RPQualityConfigurationData).ToList();
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Routing Product Quality Data For DB Apply is done", null);
        }

        protected override PrepareRPQualityDataForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareRPQualityDataForDBApplyInput
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
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