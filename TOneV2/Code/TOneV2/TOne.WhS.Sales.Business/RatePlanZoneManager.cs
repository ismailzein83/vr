using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanZoneManager
    {
        public IEnumerable<SaleZone> GetRatePlanZones(SalePriceListOwnerType ownerType, int ownerId, int? sellingNumberPlanId, DateTime effectiveOn, IEnumerable<int> countryIds, char zoneLetter, Vanrise.Entities.TextFilterType? zoneNameFilterType, string zoneNameFilter, int fromRow, int toRow)
        {
            IEnumerable<SaleZone> zones = GetFilteredZones(ownerType, ownerId, sellingNumberPlanId, effectiveOn, countryIds, zoneLetter, zoneNameFilterType, zoneNameFilter);
            return GetPagedZones(zones, fromRow, toRow);
        }

        public IEnumerable<SaleZone> GetFilteredZones(SalePriceListOwnerType ownerType, int ownerId, int? sellingNumberPlanId, DateTime effectiveOn, IEnumerable<int> countryIds, char? zoneLetter, Vanrise.Entities.TextFilterType? zoneNameFilterType, string zoneNameFilter)
        {
            IEnumerable<SaleZone> zones = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                if (!sellingNumberPlanId.HasValue)
                    throw new ArgumentNullException("sellingNumberPlanId");
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                zones = saleZoneManager.GetSaleZones(sellingNumberPlanId.Value, effectiveOn);
            }
            else
            {
                CustomerZoneManager manager = new CustomerZoneManager();
                zones = manager.GetCustomerSaleZones(ownerId, effectiveOn, false);
            }

            return GetMatchedZones(zones, countryIds, zoneLetter, zoneNameFilterType, zoneNameFilter);
        }

        public BusinessEntity.Entities.SaleEntityService GetZoneInheritedService(GetZoneInheritedServiceInput input)
        {
            SaleEntityService inheritedService;
            
            var draftManager = new RatePlanDraftManager();
            draftManager.SaveDraft(input.OwnerType, input.OwnerId, input.NewDraft);

            var ratePlanServiceLocator = new SaleEntityServiceLocator(new RatePlanServiceReadWithCache(input.OwnerType, input.OwnerId, input.EffectiveOn, input.NewDraft));

            if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
            {
                inheritedService = ratePlanServiceLocator.GetSellingProductZoneService(input.OwnerId, input.ZoneId);
            }
            else
            {
                var ratePlanManager = new RatePlanManager();
                int sellingProductId = ratePlanManager.GetSellingProductId(input.OwnerId, input.EffectiveOn, false);
                inheritedService = ratePlanServiceLocator.GetCustomerZoneService(input.OwnerId, sellingProductId, input.ZoneId);
            }

            return inheritedService;
        }

        private IEnumerable<SaleZone> GetMatchedZones(IEnumerable<SaleZone> zones, IEnumerable<int> countryIds, char? zoneLetter, Vanrise.Entities.TextFilterType? zoneNameFilterType, string zoneNameFilter)
        {
            if (zones == null)
                return null;

            if (zoneNameFilterType.HasValue && zoneNameFilter == null)
                throw new ArgumentNullException("zoneNameFilter");
            if (zoneNameFilter != null && !zoneNameFilterType.HasValue)
                throw new ArgumentNullException("zoneNameFilterType");

            return zones.FindAllRecords
            (
                x => (countryIds == null || countryIds.Contains(x.CountryId))
                && (!zoneLetter.HasValue || (x.Name != null && x.Name.Length > 0 && char.ToLower(x.Name.ElementAt(0)) == char.ToLower(zoneLetter.Value)))
                && (!zoneNameFilterType.HasValue || Vanrise.Common.Utilities.IsTextMatched(x.Name, zoneNameFilter, zoneNameFilterType.Value))
            );
        }

        private IEnumerable<SaleZone> GetPagedZones(IEnumerable<SaleZone> zones, int fromRow, int toRow)
        {
            if (zones == null)
                return zones;

            List<SaleZone> pagedZones = null;
            int count = zones.Count();

            if (count >= fromRow)
            {
                pagedZones = new List<SaleZone>();

                for (int i = fromRow - 1; i < count && i < toRow; i++)
                    pagedZones.Add(zones.ElementAt(i));
            }

            return pagedZones;
        }
    }
}
