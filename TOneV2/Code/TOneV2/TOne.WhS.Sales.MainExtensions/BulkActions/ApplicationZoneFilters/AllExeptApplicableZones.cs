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
    public class AllExeptApplicableZones : BulkActionZoneFilter
    {
        public override Guid ConfigId { get { return new Guid("06637B7A-D40A-43D2-9930-20031BBA2108"); } }

        public Dictionary<int, CountryZones> ExceptCountryZones { get; set; }

        public IEnumerable<long> ExceptZoneIds { get; set; }

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
                CountryZones exceptCountryZones;

                if (ExceptCountryZones != null && ExceptCountryZones.TryGetValue(saleZone.CountryId, out exceptCountryZones))
                {
                    if (exceptCountryZones.ExcludedZoneIds == null || !exceptCountryZones.ExcludedZoneIds.Contains(saleZone.SaleZoneId))
                        return false;
                }
                else if (ExceptZoneIds.Contains(saleZone.SaleZoneId))
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
}
