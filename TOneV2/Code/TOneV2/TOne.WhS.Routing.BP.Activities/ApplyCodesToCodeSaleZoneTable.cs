using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{

    public class ApplyCodesToCodeSaleZoneTableInput
    {
        public BaseQueue<Object> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }
    }


    public sealed class ApplyCodesToCodeSaleZoneTable : DependentAsyncActivity<ApplyCodesToCodeSaleZoneTableInput>
    {

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(ApplyCodesToCodeSaleZoneTableInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            //ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
            //dataManager.DatabaseId = inputArgument.RoutingDatabaseId;

            //DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            //{
            //    bool hasItem = false;
            //    do
            //    {
            //        hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
            //        {
            //            dataManager.ApplyCodeMatchesForDB(preparedCodeMatch);
            //        });
            //    } while (!ShouldStop(handle) && hasItem);
            //});
        }

        protected override ApplyCodesToCodeSaleZoneTableInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            throw new NotImplementedException();
        }
    }
}
