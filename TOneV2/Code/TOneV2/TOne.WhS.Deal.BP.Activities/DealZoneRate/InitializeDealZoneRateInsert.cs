using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Deal.BP.Activities
{
	public class InitializeDealZoneRateInsertInput
	{
		public IEnumerable<int> DealIdsToKeep { get; set; }
	}

	public sealed class InitializeDealZoneRateInsert : BaseAsyncActivity<InitializeDealZoneRateInsertInput>
	{
		[RequiredArgument]
		public InArgument<IEnumerable<int>> DealIdsToKeep { get; set; }

		protected override void DoWork(InitializeDealZoneRateInsertInput inputArgument, AsyncActivityHandle handle)
		{
			var dealIdsToKeep = inputArgument.DealIdsToKeep;
			var dealZoneRateManager = new DealZoneRateManager();
			dealZoneRateManager.InitializeDealZoneRateInsert(dealIdsToKeep);

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Initializing deal zone rates insert is done.");
		}

		protected override InitializeDealZoneRateInsertInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new InitializeDealZoneRateInsertInput
			{
				DealIdsToKeep = this.DealIdsToKeep.Get(context)
			};
		}
	}
}