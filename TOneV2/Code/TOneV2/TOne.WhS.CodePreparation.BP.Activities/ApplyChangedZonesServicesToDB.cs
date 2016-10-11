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
    public class ApplyChangedZonesServicesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public class ApplyChangedZonesServicesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyChangedZonesServicesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangedZonesServicesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {

            long processInstanceID = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            IChangedSaleZoneServicesDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleZoneServicesDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedChangedZonesServices) =>
                    {
                        dataManager.ApplyChangedZonesServicesToDB(preparedChangedZonesServices);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangedZonesServicesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangedZonesServicesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
