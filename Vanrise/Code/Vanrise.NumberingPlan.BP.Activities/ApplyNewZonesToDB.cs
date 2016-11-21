using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess;
using Vanrise.NumberingPlan.Data;
using Vanrise.Queueing;
namespace Vanrise.NumberingPlan.BP.Activities
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
            INewSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<INewSaleZoneDataManager>();
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZones) =>
                    {
                        dataManager.ApplyNewZonesToDB(preparedZones);
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
