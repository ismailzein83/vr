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
    public class ApplyChangedZonesToDBInput
    {
        public BaseQueue<Object> InputQueue { get; set; }
    }

    public sealed class ApplyChangedZonesToDB : Vanrise.BusinessProcess.DependentAsyncActivity<ApplyChangedZonesToDBInput>
    {
        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        protected override void DoWork(ApplyChangedZonesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            IChangedSaleZoneDataManager dataManager = CodePrepDataManagerFactory.GetDataManager<IChangedSaleZoneDataManager>();

            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedZones) =>
                    {
                        dataManager.ApplyChangedZonesToDB(preparedZones);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
        }

        protected override ApplyChangedZonesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyChangedZonesToDBInput
            {
                InputQueue = this.InputQueue.Get(context)
            };
        }
    }
}
