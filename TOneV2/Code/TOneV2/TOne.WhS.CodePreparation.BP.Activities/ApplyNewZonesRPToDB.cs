using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.WhS.CodePreparation.Entities.Processing;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyNewZonesRoutingProductsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyNewZonesRoutingProductsToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyNewZonesRoutingProductsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyNewZonesRoutingProductsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            INewSaleZoneRoutingProductDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleZoneRoutingProductDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZonesRoutingProducts) =>
                    {
                        dataManager.ApplyNewZonesRoutingProductsToDB(preparedZonesRoutingProducts);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewZonesRoutingProductsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewZonesRoutingProductsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
