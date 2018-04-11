using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.WhS.Deal.BP.Activities
{
	public class DealZoneRateBatch
	{
		public List<DealZoneRate> DealZoneRates { get; set; }
	}

	public class GenerateInput
	{
		public DealZoneRateByZoneGroup NewDealZoneRatesByZoneGroup { get; set; }
		public BaseQueue<DealZoneRateBatch> OutputQueue { get; set; }
	}

	public sealed class GenerateDealZoneRatesForApply : BaseAsyncActivity<GenerateInput>
	{
		[RequiredArgument]
		public InArgument<DealZoneRateByZoneGroup> NewDealZoneRatesByZoneGroup { get; set; }

		[RequiredArgument]
		public InOutArgument<BaseQueue<DealZoneRateBatch>> OutputQueue { get; set; }

		protected override void DoWork(GenerateInput inputArgument, AsyncActivityHandle handle)
		{
			if (inputArgument.NewDealZoneRatesByZoneGroup == null)
				return;
				
			DealZoneRateBatch dealZoneRateBatch = new DealZoneRateBatch();
			dealZoneRateBatch.DealZoneRates = new List<DealZoneRate>();

			foreach (var dealZoneRatesByZoneGroup in inputArgument.NewDealZoneRatesByZoneGroup)
			{
				foreach (var dealZoneRatesByZoneId in dealZoneRatesByZoneGroup.Value)
				{
					foreach (var dealZoneRatesByTierId in dealZoneRatesByZoneId.Value)
					{
						dealZoneRateBatch.DealZoneRates.AddRange(dealZoneRatesByTierId.Value);
						if (dealZoneRateBatch.DealZoneRates.Count >= 10000)
						{
							inputArgument.OutputQueue.Enqueue(dealZoneRateBatch);
							dealZoneRateBatch = new DealZoneRateBatch();
							dealZoneRateBatch.DealZoneRates = new List<DealZoneRate>();
						}
					}
				}
			}

			if (dealZoneRateBatch.DealZoneRates.Count > 0)
			{
				inputArgument.OutputQueue.Enqueue(dealZoneRateBatch);
				dealZoneRateBatch = new DealZoneRateBatch();
				dealZoneRateBatch.DealZoneRates = new List<DealZoneRate>();
			}

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Generating deal zone rates for applying to database is done.");
		}

		protected override GenerateInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new GenerateInput()
			{
				NewDealZoneRatesByZoneGroup = this.NewDealZoneRatesByZoneGroup.Get(context),
				OutputQueue = this.OutputQueue.Get(context)
			};
		}

		protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
		{
			if (this.OutputQueue.Get(context) == null)
				this.OutputQueue.Set(context, new MemoryQueue<DealZoneRateBatch>());
			base.OnBeforeExecute(context, handle);
		}
	}
}