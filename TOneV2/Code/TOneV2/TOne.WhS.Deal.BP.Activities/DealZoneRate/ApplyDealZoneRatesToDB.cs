using System;
using System.Activities;
using TOne.WhS.Deal.Data;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.Deal.BP.Activities
{
	public class ApplyDealZoneRatesToDBInput
	{
		public BaseQueue<Object> InputQueue { get; set; }

	}

	public sealed class ApplyDealZoneRatesToDB : DependentAsyncActivity<ApplyDealZoneRatesToDBInput>
	{
		[RequiredArgument]
		public InArgument<BaseQueue<Object>> InputQueue { get; set; }
		protected override void DoWork(ApplyDealZoneRatesToDBInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
		{
			IDealZoneRateDataManager dataManager = DealDataManagerFactory.GetDataManager<IDealZoneRateDataManager>();

			DoWhilePreviousRunning(previousActivityStatus, handle, () =>
			{
				bool hasItem = false;
				do
				{
					hasItem = inputArgument.InputQueue.TryDequeue((preparedDealZoneRate) =>
					{
						dataManager.ApplyNewDealZoneRatesToDB(preparedDealZoneRate);
					});
				} while (!ShouldStop(handle) && hasItem);
			});

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information,"Applying deal zone rates to database is done.");
		}

		protected override ApplyDealZoneRatesToDBInput GetInputArgument2(AsyncCodeActivityContext context)
		{
			return new ApplyDealZoneRatesToDBInput()
			{
				InputQueue = this.InputQueue.Get(context)
			};
		}
	}
}
