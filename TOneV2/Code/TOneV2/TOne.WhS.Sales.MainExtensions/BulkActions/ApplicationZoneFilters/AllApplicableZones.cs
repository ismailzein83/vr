using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class AllApplicableZones : BulkActionZoneFilter
    {
        public override Guid ConfigId { get { return new Guid("BDC22FEB-14E1-4F0D-8C3E-EF54A5A36312"); } }

        public override IEnumerable<long> GetApplicableZoneIds(IApplicableZoneIdsContext context)
        {
			if (context.SaleZones == null || context.SaleZones.Count() == 0)
				return null;//throw new MissingMemberException("SaleZones");

            Func<int, long, bool, SaleEntityZoneRate> getSellingProductZoneRate;
            Func<int, int, long, bool, SaleEntityZoneRate> getCustomerZoneRate;
            Func<int, long, SaleEntityZoneRoutingProduct> getSellingProductZoneCurrentRP;
            Func<int, int, long, SaleEntityZoneRoutingProduct> getCustomerZoneCurrentRP;
            Dictionary<int, DateTime> countryBEDsByCountryId;
            Dictionary<int, DateTime> countryEEDsByCountryId;

            UtilitiesManager.SetBulkActionContextHelpers(context.OwnerType, context.OwnerId, context.GetSellingProductZoneRate, context.GetCustomerZoneRate, out getSellingProductZoneRate, out getCustomerZoneRate, out getSellingProductZoneCurrentRP, out getCustomerZoneCurrentRP, out countryBEDsByCountryId, out countryEEDsByCountryId);

            List<long> applicableZoneIds = new List<long>();

            foreach (SaleZone saleZone in context.SaleZones)
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
