using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using TOne.Business;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes
    public class PrepareRouteDetailsForDBApplyInput
    {
        public BaseQueue<RouteDetailBatch> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }

        public BaseQueue<Object> OutputQueue { get; set; }
    }

    #endregion


    public sealed class PrepareRouteDetailsForDBApply : DependentAsyncActivity<PrepareRouteDetailsForDBApplyInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<RouteDetailBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<Object>> OutputQueue { get; set; }

        protected override void DoWork(PrepareRouteDetailsForDBApplyInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int batchSize = ConfigParameterManager.Current.GetBCPBatchSize();
            IRouteDetailDataManager dataManager = LCRDataManagerFactory.GetDataManager<IRouteDetailDataManager>();
            dataManager.DatabaseId = inputArgument.RoutingDatabaseId;
            List<RouteDetail> routeDetails = new List<RouteDetail>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItems = false;
                do
                {
                    hasItems = inputArgument.InputQueue.TryDequeue(
                    (routeDetailBatch) =>
                    {
                        routeDetails.AddRange(routeDetailBatch.Routes);
                        if (routeDetails.Count >= batchSize)
                        {
                            Object preparedRouteDetails = dataManager.PrepareRouteDetailsForDBApply(routeDetails);
                            inputArgument.OutputQueue.Enqueue(preparedRouteDetails);
                            routeDetails = new List<RouteDetail>();
                        }
                    });
                } while (!ShouldStop(handle) && hasItems);

            });
            if (routeDetails.Count > 0)
            {
                Object preparedRouteDetails = dataManager.PrepareRouteDetailsForDBApply(routeDetails);
                inputArgument.OutputQueue.Enqueue(preparedRouteDetails);
            }
        }

        protected override PrepareRouteDetailsForDBApplyInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new PrepareRouteDetailsForDBApplyInput()
            {
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<object>());
            base.OnBeforeExecute(context, handle);
        }
    }
}
