using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public class ApplyCodeMatchesToDBInput
    {
		public bool ShouldApplyCodeZoneMatch { get; set; }

        public BaseQueue<Object> InputQueue { get; set; }

        public int RoutingDatabaseId { get; set; }
    }


    public sealed class ApplyCodeMatchesToDB : DependentAsyncActivity<ApplyCodeMatchesToDBInput>
    {
		[RequiredArgument]
		public InArgument<bool> ShouldApplyCodeZoneMatch { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<Object>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void DoWork(ApplyCodeMatchesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            ICodeMatchesDataManager dataManager = RoutingDataManagerFactory.GetDataManager<ICodeMatchesDataManager>();
			dataManager.ShouldApplyCodeZoneMatch = inputArgument.ShouldApplyCodeZoneMatch;
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);
            
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                bool hasItem = false;
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((preparedCodeMatch) =>
                    {
                        dataManager.ApplyCodeMatchesForDB(preparedCodeMatch);
                    });
                } while (!ShouldStop(handle) && hasItem);
            });
            handle.SharedInstanceData.WriteTrackingMessage(LogEntryType.Information, "Applying Code Matches To DB is done", null);
        }

        protected override ApplyCodeMatchesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ApplyCodeMatchesToDBInput()
            {
				ShouldApplyCodeZoneMatch = this.ShouldApplyCodeZoneMatch.Get(context),
                InputQueue = this.InputQueue.Get(context),
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }
    }
}
