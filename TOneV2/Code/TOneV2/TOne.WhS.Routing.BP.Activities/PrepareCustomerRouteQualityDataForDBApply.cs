using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;
using System.Linq;
using System.Collections.Generic;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareCustomerRouteQualityDataForDBApplyInput
    {
        public int RoutingDatabaseId { get; set; }
        public BaseQueue<RouteRuleQualityConfigurationDataBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareCustomerRouteQualityDataForDBApply : DependentAsyncActivity<PrepareCustomerRouteQualityDataForDBApplyInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RouteRuleQualityConfigurationDataBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareCustomerRouteQualityDataForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICustomerQualityConfigurationDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICustomerQualityConfigurationDataManager>();
            dataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            PrepareDataForDBApply(previousActivityStatus, handle, dataManager, inputArgument.InputQueue, inputArgument.OutputQueue, (batch) =>
            {
                return batch.RoutingQualityConfigurationDataList.Select(itm => itm as CustomerRouteQualityConfigurationData).ToList();
            });

            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Customer Route Quality Data For DB Apply is done", null);
        }

        protected override PrepareCustomerRouteQualityDataForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareCustomerRouteQualityDataForDBApplyInput
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