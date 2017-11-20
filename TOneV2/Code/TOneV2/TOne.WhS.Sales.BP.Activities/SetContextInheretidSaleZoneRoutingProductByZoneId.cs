using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
   public class SetContextInheretidSaleZoneRoutingProductByZoneId: CodeActivity
    {
        //#region Input Arguments

        //[RequiredArgument]
        //public InArgument<DateTime> MinimumActionDate { get; set; }

        //#endregion
        //protected override void Execute(CodeActivityContext context)
        //{
        //    var ratePlanContext = context.GetRatePlanContext() as RatePlanContext;

        //    if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
        //        return;

        //    int sellingProductId = new CarrierAccountManager().GetSellingProductId(ratePlanContext.OwnerId);
        //    IEnumerable<long> saleZoneIds = ratePlanContext.ExistingZonesByCountry.SelectMany(x => x.Value).MapRecords(x => x.ZoneId);

        //    DateTime minimumActionDate = MinimumActionDate.Get(context);
        //    DateTime minimumDate = Utilities.Min(minimumActionDate, DateTime.Today);

        //    IEnumerable<SaleZoneRoutingProduct> zoneRoutingProducts = new SaleEntityRoutingProductManager().GetExistingZoneRoutingProductsByZoneIds(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneIds, minimumDate);
        //    ratePlanContext.InheritedZoneRoutingProductsByZoneId = StructureZoneRoutingProductsByZoneId(zoneRoutingProducts);
        //}
        //#region Private Methods

        //private InheritedZoneRoutingProductsByZoneId StructureZoneRoutingProductsByZoneId(IEnumerable<SaleZoneRoutingProduct> routingProducts)
        //{
        //    var routingProductsByZoneId = new InheritedZoneRoutingProductsByZoneId();

        //    if (routingProducts != null)
        //    {
        //        foreach (SaleZoneRoutingProduct SaleZoneRoutingProduct in routingProducts.OrderBy(x => x.BED))
        //        {
        //            ZoneInheritedRoutingProducts zoneRoutingProducts;

        //            if (!routingProductsByZoneId.TryGetValue(SaleZoneRoutingProduct.SaleZoneId, out zoneRoutingProducts))
        //            {
        //                zoneRoutingProducts = new ZoneInheritedRoutingProducts();
        //                routingProductsByZoneId.Add(SaleZoneRoutingProduct.SaleZoneId, zoneRoutingProducts);
        //            }
        //                zoneRoutingProducts.RoutingProducts.Add(SaleZoneRoutingProduct);
        //        }
        //    }

        //    return routingProductsByZoneId;
        //}

        //#endregion
    }
}
