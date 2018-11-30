using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class SaleZoneMatchingSupplierDealFilter : ISaleZoneFilter
    {
        public int SupplierDealId { get; set; }
        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            if (context.CustomData == null)
            {
                var deal = new DealDefinitionManager().GetDealDefinition(SupplierDealId);
                var supplierZoneIds = deal.Settings.GetDealSupplierZoneIds();
                context.CustomData = new CodeZoneMatchManager().GetSaleZonesMatchedToSupplierZones(supplierZoneIds);
            }

            foreach (var saleZoneMatch in (IEnumerable<CodeSaleZoneMatch>)context.CustomData)
            {
                if (saleZoneMatch.SaleZoneId == context.SaleZone.SaleZoneId)
                    return false;
            }

            return true;
        }
    }
}
