using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetZonesWithMissingRates : CodeActivity
    {

        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        #endregion

        #region Output Arguments
        [RequiredArgument]
        public OutArgument<List<long>> ZoneIdsWithMissingRates { get; set; }
        #endregion
        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            RatePlanDraftManager ratePlanDraftManager = new RatePlanDraftManager();
            IEnumerable<ExistingZone> effectiveZonesAfterProcessDate = ratePlanContext.EffectiveAndFutureExistingZonesByCountry.SelectMany(x => x.Value).ToList();
            
            IEnumerable<RateToChange> ratesToChange = this.RatesToChange.Get(context);
            Dictionary<long, RateToChange> NormalRatesByZoneId = StructureNormalRatesToByZoneId(ratesToChange);

            HashSet<int> countryIdsWithDefaultRates = new HashSet<int>();
            Dictionary<int, List<long>> zoneIdsWithDefaultRatesByCountryIds = new Dictionary<int, List<long>>();
            List<long> zoneIdsWithMissingRates = new List<long>();
            foreach (var existingZone in effectiveZonesAfterProcessDate)
            {
                RateToChange rateToChange = (NormalRatesByZoneId.Count() != 0) ? NormalRatesByZoneId.GetRecord(existingZone.ZoneId) : null;
                if (rateToChange == null)
                {
                    List<long> existingZones = zoneIdsWithDefaultRatesByCountryIds.GetOrCreateItem(existingZone.CountryId);
                    existingZones.Add(existingZone.ZoneId);
                    zoneIdsWithMissingRates.Add(existingZone.ZoneId);
                }
            }

            ZoneIdsWithMissingRates.Set(context, zoneIdsWithMissingRates);
            SellingZonesWithDefaultRatesTaskData sellingZonesWithDefaultRatesTaskData = new SellingZonesWithDefaultRatesTaskData()
            {
                ZoneIdsWithDefaultRatesByCountryIds = zoneIdsWithDefaultRatesByCountryIds
            };
            DraftTaskData draftTaskData = new DraftTaskData()
            {
                SellingZonesWithDefaultRatesTaskData = sellingZonesWithDefaultRatesTaskData
            };
            ratePlanDraftManager.InsertOrUpdateDraftTaskData(BusinessEntity.Entities.SalePriceListOwnerType.SellingProduct, ratePlanContext.OwnerId, draftTaskData, RatePlanStatus.Draft);

        }

        private Dictionary<long, RateToChange> StructureNormalRatesToByZoneId(IEnumerable<RateToChange> ratesToChange)
        {
            var normalRatesByZoneId = new Dictionary<long, RateToChange>();

            foreach (RateToChange rateToChange in ratesToChange)
            {
                if (rateToChange.RateTypeId == null)
                {
                    if (!normalRatesByZoneId.ContainsKey(rateToChange.ZoneId))
                        normalRatesByZoneId.Add(rateToChange.ZoneId, rateToChange);
                }
            }

            return normalRatesByZoneId;
        }
    }
}
