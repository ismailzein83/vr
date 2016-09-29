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

        // This method is invoked when the user clicks the 'Reset to default' link to reset the ZONE services of an OWNER
        public BusinessEntity.Entities.SaleEntityService GetZoneInheritedService(SalePriceListOwnerType ownerType, int ownerId, long zoneId, DateTime effectiveOn)
        {
            SaleEntityService inheritedService;
            
            var draftManager = new RatePlanDraftManager();
            Changes draft = draftManager.GetDraft(ownerType, ownerId);

            var effectiveServiceLocator = new SaleEntityServiceLocator(new EffectiveSaleEntityServiceReadWithCache(ownerType, ownerId, effectiveOn, draft));

            // The owner's draft MUST have a DraftResetZoneService on zoneId for the below method to return a correct result
            // This is assumed because the draft is saved before calling GetZoneInheritedService

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                inheritedService = effectiveServiceLocator.GetSellingProductZoneService(ownerId, zoneId);
            }
            else
            {
                var ratePlanManager = new RatePlanManager();
                int sellingProductId = ratePlanManager.GetSellingProductId(ownerId, effectiveOn, false);
                inheritedService = effectiveServiceLocator.GetCustomerZoneService(ownerId, sellingProductId, zoneId);
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
