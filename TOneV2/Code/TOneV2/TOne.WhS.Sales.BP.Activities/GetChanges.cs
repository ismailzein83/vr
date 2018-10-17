using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
	public class GetChanges : CodeActivity
	{
		#region Arguments

		[RequiredArgument]
		public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

		[RequiredArgument]
		public InArgument<int> OwnerId { get; set; }

		[RequiredArgument]
		public OutArgument<Changes> Changes { get; set; }

		#endregion

		protected override void Execute(CodeActivityContext context)
		{
			SalePriceListOwnerType ownerType = this.OwnerType.Get(context);
			int ownerId = this.OwnerId.Get(context);

			var ratePlanDraftManager = new RatePlanDraftManager();
			Changes changes = ratePlanDraftManager.GetDraft(ownerType, ownerId);
			Changes filteredChanges = null;
			if (changes != null)
			{
				filteredChanges = new Changes
				{
					CountryChanges = changes.CountryChanges,
					CurrencyId = changes.CurrencyId,
					DefaultChanges = changes.DefaultChanges,
					SubscriberOwnerEntities = changes.SubscriberOwnerEntities,
				};

				if (changes.ZoneChanges != null)
				{
					filteredChanges.ZoneChanges = new List<ZoneChanges>();
					foreach (var zoneChange in changes.ZoneChanges)
					{
						var filteredZoneChange = new ZoneChanges
						{
							ClosedService = zoneChange.ClosedService,
							CountryId = zoneChange.CountryId,
							NewOtherRateBED = zoneChange.NewOtherRateBED,
							NewRoutingProduct = zoneChange.NewRoutingProduct,
							ZoneName = zoneChange.ZoneName,
							ZoneId = zoneChange.ZoneId,
							RoutingProductChange = zoneChange.RoutingProductChange,
							ResetService = zoneChange.ResetService,
							NewService = zoneChange.NewService,
							ProfitPerc = zoneChange.ProfitPerc
						};
						var rateTypeIds = BusinessEntity.Business.Helper.GetRateTypeIds(ownerId, zoneChange.ZoneId, DateTime.Now);
						if (zoneChange.NewRates != null)
						{
							var filteredNewRates = new List<DraftRateToChange>();
							foreach (var newRate in zoneChange.NewRates)
							{
								if (newRate.RateTypeId == null || (rateTypeIds != null && rateTypeIds.Any(item => item == newRate.RateTypeId)))
									filteredNewRates.Add(newRate);
							}
							filteredZoneChange.NewRates = filteredNewRates;
						}
						if (zoneChange.ClosedRates != null)
						{
							var filteredClosedRates = new List<DraftRateToClose>();
							foreach (var closedRate in zoneChange.ClosedRates)
							{
								if (closedRate.RateTypeId == null || (rateTypeIds != null && rateTypeIds.Any(item => item == closedRate.RateTypeId)))
									filteredClosedRates.Add(closedRate);
							}
							filteredZoneChange.ClosedRates = filteredClosedRates;
						}
						filteredChanges.ZoneChanges.Add(filteredZoneChange);
					}
				}
			}


			this.Changes.Set(context, filteredChanges);
		}
	}
}
