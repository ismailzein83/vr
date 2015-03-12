using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.LCR.Entities;
using TOne.LCR.Business;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class BuildRoutesInput
    {
        public ZoneCustomerRates CustomerZoneRates { get; set; }

        public SupplierZoneRates SupplierZoneRates { get; set; }
        
        public BaseQueue<SingleDestinationCodeMatches> InputQueue { get; set; }

        public BaseQueue<RouteDetailBatch> OutputQueue { get; set; }
    }

    #endregion

    public sealed class BuildRoutes : DependentAsyncActivity<BuildRoutesInput>
    {
        [RequiredArgument]
        public InArgument<ZoneCustomerRates> CustomerZoneRates { get; set; }

        [RequiredArgument]
        public InArgument<SupplierZoneRates> SupplierZoneRates { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<SingleDestinationCodeMatches>> InputQueue { get; set; }
        
        [RequiredArgument]
        public InOutArgument<BaseQueue<RouteDetailBatch>> OutputQueue { get; set; }

        protected override BuildRoutesInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new BuildRoutesInput
            {
                CustomerZoneRates = this.CustomerZoneRates.Get(context),
                SupplierZoneRates = this.SupplierZoneRates.Get(context),
                InputQueue = this.InputQueue.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<RouteDetailBatch>());
            base.OnBeforeExecute(context, handle);
        }

        protected override void DoWork(BuildRoutesInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((singleDestinationCodeMatches) =>
                    {
                        using(var context = new SingleDestinationRoutesBuildContext(singleDestinationCodeMatches, 
                            inputArgument.CustomerZoneRates, inputArgument.SupplierZoneRates, null, null))
                        {
                            var routes = context.BuildRoutes();
                            if (routes != null && routes.Count > 0)
                            {
                                inputArgument.OutputQueue.Enqueue(new RouteDetailBatch
                                    {
                                        Routes = routes
                                    });
                            }
                        }
                    });
                    //MemoryQueue<RouteDetailBatch> outputQueue = inputArgument.OutputQueue as MemoryQueue<RouteDetailBatch>;
                    // if (outputQueue != null)
                    // {
                    //     while (outputQueue.Count > 1000)
                    //         System.Threading.Thread.Sleep(1000);
                    // }
                }
                while (!ShouldStop(handle) && hasItem);
            });
        }
    }
}
