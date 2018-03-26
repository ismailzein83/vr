using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Deal.BP.Activities
{
	public class GenerateDealZoneRatesInput
	{
		public Boolean IsSale { get; set; }
		public IEnumerable<DealDefinition> DealDefinitionsToReevaluate { get; set; }
	}

	public class GenerateDealZoneRatesOutput
	{
		public DealZoneRateByZoneGroup NewDealZoneRateByZoneGroup { get; set; }
	}

	public sealed class GenerateDealZoneRates : BaseAsyncActivity<GenerateDealZoneRatesInput, GenerateDealZoneRatesOutput>
	{
		[RequiredArgument]
		public InArgument<Boolean> IsSale { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<DealDefinition>> DealDefinitionsToReevaluate { get; set; }

		[RequiredArgument]
		public OutArgument<DealZoneRateByZoneGroup> NewDealZoneRateByZoneGroup { get; set; }

		protected override GenerateDealZoneRatesOutput DoWorkWithResult(GenerateDealZoneRatesInput inputArgument, AsyncActivityHandle handle)
		{
			var isSale = inputArgument.IsSale;
			var dealDefinitionsToReevaluate = inputArgument.DealDefinitionsToReevaluate;
			var dealDefinitionManager = new DealDefinitionManager();
			var dealZoneRatesByZoneGroup = new DealZoneRateByZoneGroup();

			if (isSale)
			{
				var dealSaleZoneGroups = dealDefinitionManager.GetDealSaleZoneGroupsByDealDefinitions(dealDefinitionsToReevaluate);
				if (dealSaleZoneGroups != null)
				{
					foreach (var dealSaleZoneGroup in dealSaleZoneGroups)
					{
						var dealZoneGroup = new DealZoneGroup() { DealId = dealSaleZoneGroup.DealId, ZoneGroupNb = dealSaleZoneGroup.DealSaleZoneGroupNb };
						var dealZoneRatesByZone = dealZoneRatesByZoneGroup.GetOrCreateItem(dealZoneGroup);

						foreach (var tier in dealSaleZoneGroup.Tiers)
						{
							BuildDealZoneRates(dealSaleZoneGroup.DealId, dealSaleZoneGroup.DealSaleZoneGroupNb, isSale, dealZoneRatesByZone, tier.TierNumber, tier.RatesByZoneId);
						}
					}
				}
			}

			else
			{
				var dealSupplierZoneGroups = dealDefinitionManager.GetDealSupplierZoneGroupsByDealDefinitions(dealDefinitionsToReevaluate);
				if (dealSupplierZoneGroups != null)
				{
					foreach (var dealSupplierZoneGroup in dealSupplierZoneGroups)
					{
						var dealZoneGroup = new DealZoneGroup() { DealId = dealSupplierZoneGroup.DealId, ZoneGroupNb = dealSupplierZoneGroup.DealSupplierZoneGroupNb };
						var dealZoneRatesByZone = dealZoneRatesByZoneGroup.GetOrCreateItem(dealZoneGroup);

						foreach (var tier in dealSupplierZoneGroup.Tiers)
						{
							BuildDealZoneRates(dealSupplierZoneGroup.DealId, dealSupplierZoneGroup.DealSupplierZoneGroupNb, isSale, dealZoneRatesByZone, tier.TierNumber, tier.RatesByZoneId);
						}
					}
				}
			}

			handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Generating deal zone rates is done.");

			return new GenerateDealZoneRatesOutput
			{
				NewDealZoneRateByZoneGroup = dealZoneRatesByZoneGroup
			};
		}

		protected override GenerateDealZoneRatesInput GetInputArgument(AsyncCodeActivityContext context)
		{
			return new GenerateDealZoneRatesInput
			{
				IsSale = this.IsSale.Get(context),
				DealDefinitionsToReevaluate = this.DealDefinitionsToReevaluate.Get(context)
			};
		}

		protected override void OnWorkComplete(AsyncCodeActivityContext context, GenerateDealZoneRatesOutput result)
		{
			context.SetValue(this.NewDealZoneRateByZoneGroup, result.NewDealZoneRateByZoneGroup);
		}

		private void BuildDealZoneRates(int dealId, int zoneGroupNb, bool isSale, DealZoneRateByZoneId dealZoneRatesByZone, int tierNumber, Dictionary<long, List<DealRate>> ratesByZoneId)
		{
			if (ratesByZoneId == null)
				return;

			foreach (var zoneDealRates in ratesByZoneId)
			{
				var dealZoneRatesByTireNB = dealZoneRatesByZone.GetOrCreateItem(zoneDealRates.Key);
				var dealZoneRateList = dealZoneRatesByTireNB.GetOrCreateItem(tierNumber);
				foreach (var dealRate in zoneDealRates.Value)
				{
					dealZoneRateList.Add(new DealZoneRate()
					{
						DealId = dealId,
						ZoneGroupNb = zoneGroupNb,
						IsSale = isSale,
						TierNb = tierNumber,
						ZoneId = zoneDealRates.Key,
						Rate = dealRate.Rate,
						CurrencyId = dealRate.CurrencyId,
						BED = dealRate.BED,
						EED = dealRate.EED
					});
				}
			}
		}
	}
}
