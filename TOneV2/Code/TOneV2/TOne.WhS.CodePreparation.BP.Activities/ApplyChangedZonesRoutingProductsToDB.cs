using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyChangedZonesRoutingProductsToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyChangedZonesRoutingProductsToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyChangedZonesRoutingProductsToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangedZonesRoutingProductsToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            long processInstanceID = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            IChangedSaleZoneRoutingProductsDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleZoneRoutingProductsDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedChangedZonesRoutingProducts) =>
                    {
                        dataManager.ApplyChangedZonesRoutingProductsToDB(preparedChangedZonesRoutingProducts);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangedZonesRoutingProductsToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangedZonesRoutingProductsToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
