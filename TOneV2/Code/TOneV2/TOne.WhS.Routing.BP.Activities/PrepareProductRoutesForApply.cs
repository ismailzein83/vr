using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Queueing;

namespace TOne.WhS.Routing.BP.Activities
{
    public class PrepareProductRoutesForApplyInput
    {
        public int RoutingDatabaseId { get; set; }
        public IEnumerable<RoutingCustomerInfo> RoutingCustomerInfos { get; set; }
        public BaseQueue<RPRouteBatch> InputQueue { get; set; }
        public BaseQueue<Object> OutputQueue { get; set; }
    }

    public sealed class PrepareProductRoutesForApply : DependentAsyncActivity<PrepareProductRoutesForApplyInput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RoutingCustomerInfo>> RoutingCustomerInfos { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RPRouteBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }


        protected override void DoWork(PrepareProductRoutesForApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IRPRouteDataManager productRoutesDataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            productRoutesDataManager.RoutingDatabase = new RoutingDatabaseManager().GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            bool generateCostAnalysisByCustomer = new ConfigManager().GetProductRouteBuildGenerateCostAnalysisByCustomer();
            if (generateCostAnalysisByCustomer)
                productRoutesDataManager.RoutingCustomerInfo = inputArgument.RoutingCustomerInfos;

            PrepareDataForDBApply(previousActivityStatus, handle, productRoutesDataManager, inputArgument.InputQueue, inputArgument.OutputQueue, ProductRoutesBatch => ProductRoutesBatch.RPRouteByCustomers);
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Preparing Product Routes For Apply is done", null);
        }

        protected override PrepareProductRoutesForApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareProductRoutesForApplyInput
            {
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                RoutingCustomerInfos = this.RoutingCustomerInfos.Get(context)
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