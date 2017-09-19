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
            var applicableZoneIds = new List<long>();

            Func<int, long, bool, SaleEntityZoneRate> getSellingProductZoneRate;
            Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate;
            Func<int, long, SaleEntityZoneRoutingProduct> getSellingProductZoneCurrentRP;
            Func<int, int, long, SaleEntityZoneRoutingProduct> getCustomerZoneCurrentRP;

            UtilitiesManager.SetBulkActionContextHelpers(context.GetSellingProductZoneRate, context.GetCustomerZoneRate, out getSellingProductZoneRate, out getCustomerZoneRate, out getSellingProductZoneCurrentRP, out getCustomerZoneCurrentRP);

            foreach (SaleZone saleZone in context.SaleZones)
            {
                if (ExceptZoneIds.Contains(saleZone.SaleZoneId))
                    continue;

                CountryZones exceptCountryZones;

                if (ExceptCountryZones != null && ExceptCountryZones.TryGetValue(saleZone.CountryId, out exceptCountryZones))
                {
                    if (exceptCountryZones.ExcludedZoneIds == null || !exceptCountryZones.ExcludedZoneIds.Contains(saleZone.SaleZoneId))
                        continue;
                }

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
                    GetCustomerZoneRate = getCustomerZoneRate
                };

                if (!UtilitiesManager.IsActionApplicableToZone(isActionApplicableToZoneInput))
                    continue;

                applicableZoneIds.Add(saleZone.SaleZoneId);
            }

            return applicableZoneIds;
        }

        public class CountryZones
        {
            public int CountryId { get; set; }

            public IEnumerable<long> ExcludedZoneIds { get; set; }
        }
    }
}
