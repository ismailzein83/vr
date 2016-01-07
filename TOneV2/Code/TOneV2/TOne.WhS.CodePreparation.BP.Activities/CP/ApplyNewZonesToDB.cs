using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Data;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class ApplyNewZonesToDBInput
    {
        public int SellingNumberPlanId { get; set; }
        public BaseQueue<Object> InputQueue { get; set; }
    }
    public sealed class ApplyNewZonesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyNewZonesToDBInput>
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }
        
        protected override void DoWork(ApplyNewZonesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            int sellingNumberPlanId = inputArgument.SellingNumberPlanId;
            long processInstanceID = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
            INewSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleZoneDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZones) =>
                    {
                        dataManager.ApplyNewZonesToDB(preparedZones, sellingNumberPlanId, processInstanceID);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyNewZonesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyNewZonesToDBInput
            {
                SellingNumberPlanId = this.SellingNumberPlanId.Get(context),
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
