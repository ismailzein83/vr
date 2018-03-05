using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.MainExtensions
{
    public class SpecificApplicableZones : BulkActionZoneFilter
    {
        public override Guid ConfigId { get { return new Guid("61D047D6-DF3D-4D74-9C2C-7CEA2907C2B3"); } }

        public Dictionary<int, CountryZones> CountryZonesByCountry { get; set; }

        public IEnumerable<long> IncludedZoneIds { get; set; }

        public override IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context)
        {
            if (context.SaleZones == null || context.SaleZones.Count() == 0)
                throw new Vanrise.Entities.MissingArgumentValidationException("saleZones");

            var applicableZoneIds = new List<long>();

            Func<int, long, bool, SaleEntityZoneRate> getSellingProductZoneRate;
            Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate;
            Func<int, long, SaleEntityZoneRoutingProduct> getSellingProductZoneCurrentRP;
            Func<int, int, long, SaleEntityZoneRoutingProduct> getCustomerZoneCurrentRP;
            Dictionary<int, DateTime> countryBEDsByCountryId;
            Dictionary<int, DateTime> countryEEDsByCountryId;

            UtilitiesManager.SetBulkActionContextHelpers(context.OwnerType, context.OwnerId, context.GetSellingProductZoneRate, context.GetCustomerZoneRate, out getSellingProductZoneRate, out getCustomerZoneRate, out getSellingProductZoneCurrentRP, out getCustomerZoneCurrentRP, out countryBEDsByCountryId, out countryEEDsByCountryId);

            Func<SaleZone, bool> filterFunc = (saleZone) =>
            {
                CountryZones countryZones;

                if (CountryZonesByCountry != null && CountryZonesByCountry.TryGetValue(saleZone.CountryId, out countryZones))
                {
                    if (countryZones.ExcludedZoneIds != null && countryZones.ExcludedZoneIds.Contains(saleZone.SaleZoneId))
                        return false;
                }
                else if (IncludedZoneIds != null && !IncludedZoneIds.Contains(saleZone.SaleZoneId))
                    return false;

                return true;
            };

            IEnumerable<SaleZone> filteredZones = context.SaleZones.FindAllRecords(filterFunc);

            foreach (SaleZone saleZone in filteredZones)
            {
                var isActionApplicableToZoneInput = new BulkActionApplicableToZoneInput()
                {
                    OwnerType = context.OwnerType,
                    OwnerId = context.OwnerId,
                    SaleZone = saleZone,
                    BulkAction = context.BulkAction,
                    Draft = context.DraftData,
                    GetCurrentSellingProductZoneRP = getSellingProductZoneCurrentRP,
                    GetCurrentCustomerZoneRP = getCustomerZoneCurrentRP,
                    GetSellingProductZoneRate = getSellingProductZoneRate,
                    GetCustomerZoneRate = getCustomerZoneRate,
                    CountryBEDsByCountryId = countryBEDsByCountryId,
                    CountryEEDsByCountryId = countryEEDsByCountryId
                };

                if (UtilitiesManager.IsActionApplicableToZone(isActionApplicableToZoneInput))
                    applicableZoneIds.Add(saleZone.SaleZoneId);
            }

            return applicableZoneIds;
        }
    }

    public class CountryZones
    {
        public int CountryId { get; set; }

        public IEnumerable<long> ExcludedZoneIds { get; set; }
    }
}
