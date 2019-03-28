using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Routing.Business;

namespace TOne.WhS.Routing.MainExtensions
{
    public class SaleZoneMatchingSupplierDealFilter : ISaleZoneFilter
    {
        public List<int> SupplierDealIds { get; set; }

        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            if (context.CustomData == null && SupplierDealIds != null)
            {
                List<long> allDealSupplierZoneIds = new List<long>();
                foreach (var supplierDealId in SupplierDealIds)
                {
                    var supplierDeal = new DealDefinitionManager().GetDealDefinition(supplierDealId);
                    List<long> supplierZoneIds = supplierDeal.Settings.GetDealSupplierZoneIds();
                    if (supplierZoneIds != null)
                        allDealSupplierZoneIds.AddRange(supplierZoneIds);
                }

                context.CustomData = new CodeZoneMatchManager().GetSaleZonesMatchedToSupplierZones(allDealSupplierZoneIds, context.SellingNumberPlanId);
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