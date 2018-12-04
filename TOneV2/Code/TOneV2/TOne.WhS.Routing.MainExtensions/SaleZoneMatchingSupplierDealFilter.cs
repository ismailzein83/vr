using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.MainExtensions
{
    public class SaleZoneMatchingSupplierDealFilter : ISaleZoneFilter
    {
        public int SupplierDealId { get; set; }

        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            if (context.CustomData == null)
            {
                var supplierDeal = new DealDefinitionManager().GetDealDefinition(SupplierDealId);
                var supplierZoneIds = supplierDeal.Settings.GetDealSupplierZoneIds();
                context.CustomData = new CodeZoneMatchManager().GetSaleZonesMatchedToSupplierZones(supplierZoneIds, context.SellingNumberPlanId);
            }

            if (context.CustomData != null)
            {
                foreach (var saleZoneMatch in (IEnumerable<SaleZoneDefintion>)context.CustomData)
                {
                    if (saleZoneMatch.SaleZoneId == context.SaleZone.SaleZoneId)
                        return false;
                }
            }

            return true;
        }
    }
}
