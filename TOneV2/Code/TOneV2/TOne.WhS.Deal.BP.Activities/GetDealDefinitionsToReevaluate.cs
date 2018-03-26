using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
	public class GetDealDefinitionsToReevaluateInput
	{
		public DateTime DealEffectiveAfter { get; set; }
	}

	public class GetDealDefinitionsToReevaluateOutput
	{
		public IEnumerable<DealDefinition> DealDefinitionsToReevaluate { get; set; }
		public IEnumerable<int> DealIdsToReevaluate { get; set; }
		public IEnumerable<int> DealIdsToKeep { get; set; }
	}

	public sealed class GetDealDefinitionsToReevaluate : BaseAsyncActivity<GetDealDefinitionsToReevaluateInput, GetDealDefinitionsToReevaluateOutput>
	{

		[RequiredArgument]
		public InArgument<DateTime> DealEffectiveAfter { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<DealDefinition>> DealDefinitionsToReevaluate { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<int>> DealIdsToReevaluate { get; set; }

		[RequiredArgument]
		public OutArgument<IEnumerable<int>> DealIdsToKeep { get; set; }

		protected override GetDealDefinitionsToReevaluateOutput DoWorkWithResult(GetDealDefinitionsToReevaluateInput inputArgument, AsyncActivityHandle handle)
		{
			var dealEffectiveAfter = inputArgument.DealEffectiveAfter;

			var dealDefinitionManager = new DealDefinitionManager();
			var cachedDeals = dealDefinitionManager.GetAllCachedDealDefinitions();

			var dealDefinitionsToReevaluate = new List<DealDefinition>();
			var dealIdsToReevaluate = new List<int>();
			var dealIdsToKeep = new List<int>();

			if (cachedDeals != null)
			{
				foreach (var cachedDealKvp in cachedDeals)
				{
					var dealDefinition = cachedDealKvp.Value;
					int dealId = cachedDealKvp.Key;

					if (dealDefinition.Settings.EndDate.VRGreaterThan(dealEffectiveAfter))
					{
						dealDefinitionsToReevaluate.Add(dealDefinition);
						dealIdsToReevaluate.Add(dealId);
					}
					else
					{
						dealIdsToKeep.Add(dealId);
					}
				}
			}

			if (dealDefinitionsToReevaluate.Count == 0)
			{
				dealDefinitionsToReevaluate = null;
				dealIdsToReevaluate = null;
			}

			if (dealIdsToKeep.Count == 0)
				dealIdsToKeep = null;

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Getting Deals to reevaluate is done.");

			return new GetDealDefinitionsToReevaluateOutput()
			{
				DealDefinitionsToReevaluate = dealDefinitionsToReevaluate,
				DealIdsToReevaluate = dealIdsToReevaluate,
				DealIdsToKeep = dealIdsToKeep
			};
		}

		protected override GetDealDefinitionsToReevaluateInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new GetDealDefinitionsToReevaluateInput
			{
				DealEffectiveAfter = this.DealEffectiveAfter.Get(context)
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDealDefinitionsToReevaluateOutput result)
		{
			context.SetValue(this.DealDefinitionsToReevaluate, result.DealDefinitionsToReevaluate);
			context.SetValue(this.DealIdsToReevaluate, result.DealIdsToReevaluate);
			context.SetValue(this.DealIdsToKeep, result.DealIdsToKeep);
		}
	}
}