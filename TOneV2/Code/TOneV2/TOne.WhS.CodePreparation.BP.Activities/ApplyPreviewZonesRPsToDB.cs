using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyPreviewZonesRoutingProductsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public sealed class ApplyPreviewZonesRoutingProductsToDB : DependentAsyncActivity<ApplyPreviewZonesRoutingProductsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyPreviewZonesRoutingProductsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ISaleZoneRoutingProductPreviewDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<ISaleZoneRoutingProductPreviewDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZonesRoutingProducts) =>
                    {
                        dataManager.ApplyPreviewZonesRoutingProductsToDB(preparedZonesRoutingProducts);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyPreviewZonesRoutingProductsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyPreviewZonesRoutingProductsToDBInput()
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
