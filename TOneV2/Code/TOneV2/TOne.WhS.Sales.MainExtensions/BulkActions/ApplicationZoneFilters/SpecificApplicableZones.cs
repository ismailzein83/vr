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
            var applicableZoneIds = new List<long>();
            if (CountryZonesByCountry != null && CountryZonesByCountry.Count > 0)
            {
                var ratePlanManager = new RatePlanManager();
                int sellingNumberPlanId = ratePlanManager.GetOwnerSellingNumberPlanId(context.OwnerType, context.OwnerId);
                IEnumerable<SaleZone> ownerSaleZones = ratePlanManager.GetSaleZones(context.OwnerType, context.OwnerId, DateTime.Today, true);

                if (ownerSaleZones != null)
                {
                    Func<int, long, bool, SaleEntityZoneRate> getSellingProductZoneRate;
                    Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate;
                    Func<int, long, SaleEntityZoneRoutingProduct> getSellingProductZoneCurrentRP;
                    Func<int, int, long, SaleEntityZoneRoutingProduct> getCustomerZoneCurrentRP;

                    UtilitiesManager.SetBulkActionContextHelpers(context.GetSellingProductZoneRate, context.GetCustomerZoneRate, out getSellingProductZoneRate, out getCustomerZoneRate, out getSellingProductZoneCurrentRP, out getCustomerZoneCurrentRP);

                    var excludedSaleZoneIds = new List<long>();
                    foreach (CountryZones countryZones in CountryZonesByCountry.Values)
                    {
                        if (countryZones.ExcludedZoneIds != null)
                            excludedSaleZoneIds.AddRange(countryZones.ExcludedZoneIds);
                    }
                    IEnumerable<SaleZone> saleZonesByCountryIds = ownerSaleZones.FindAllRecords(x =>
                    {
                        if (!CountryZonesByCountry.Keys.Contains(x.CountryId))
                            return false;
                        if (excludedSaleZoneIds.Contains(x.SaleZoneId))
                            return false;
                        return true;
                    });
                    foreach (SaleZone saleZone in saleZonesByCountryIds)
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
                            GetCustomerZoneRate = getCustomerZoneRate
                        };
                        if (UtilitiesManager.IsActionApplicableToZone(isActionApplicableToZoneInput))
                            applicableZoneIds.Add(saleZone.SaleZoneId);
                    }
                }
            }
            if (IncludedZoneIds != null && IncludedZoneIds.Count() > 0)
                applicableZoneIds.AddRange(IncludedZoneIds);
            return applicableZoneIds;
        }

        public class CountryZones
        {
            public int CountryId { get; set; }

            public IEnumerable<long> ExcludedZoneIds { get; set; }
        }
    }
}
