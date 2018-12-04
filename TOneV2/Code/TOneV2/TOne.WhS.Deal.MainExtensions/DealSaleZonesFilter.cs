using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Business;

namespace TOne.WhS.Deal.MainExtensions
{
    public class DealSaleZonesFilter : ISaleZoneFilter
    {
        public int DealId { get; set; }

        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            if (context.CustomData == null)
            {
                var saleDeal = new DealDefinitionManager().GetDealDefinition(DealId);
                context.CustomData = saleDeal.Settings.GetDealSaleZoneIds();
            }

            if (context.CustomData != null)
            {
                foreach (var saleZoneId in (IEnumerable<long>)context.CustomData)
                {
                    if (saleZoneId == context.SaleZone.SaleZoneId)
                        return false;
                }
            }
            return true;
        }
    }
}