using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ProcessSaleZoneRoutingProducts : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToAdd>> SaleZoneRoutingProductsToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToClose>> SaleZoneRoutingProductsToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingSaleZoneRoutingProduct>> ExistingSaleZoneRoutingProducts { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingZone>> ExistingZones { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedSaleZoneRoutingProduct>> ChangedSaleZoneRoutingProducts { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = SaleZoneRoutingProductsToAdd.Get(context);
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = SaleZoneRoutingProductsToClose.Get(context);
            IEnumerable<ExistingSaleZoneRoutingProduct> existingSaleZoneRoutingProducts = ExistingSaleZoneRoutingProducts.Get(context);
            IEnumerable<ExistingZone> existingZones = ExistingZones.Get(context);

            var processSaleZoneRoutingProductsContext = new ProcessSaleZoneRoutingProductsContext()
            {
                SaleZoneRoutingProductsToAdd = saleZoneRoutingProductsToAdd,
                SaleZoneRoutingProductsToClose = saleZoneRoutingProductsToClose,
                ExistingSaleZoneRoutingProducts = existingSaleZoneRoutingProducts,
                ExistingZones = existingZones
            };

            var priceListSaleZoneRoutingProductManager = new PriceListSaleZoneRoutingProductManager();
            priceListSaleZoneRoutingProductManager.ProcessSaleZoneRoutingProducts(processSaleZoneRoutingProductsContext);

            NewSaleZoneRoutingProducts.Set(context, processSaleZoneRoutingProductsContext.NewSaleZoneRoutingProducts);
            ChangedSaleZoneRoutingProducts.Set(context, processSaleZoneRoutingProductsContext.ChangedSaleZoneRoutingProducts);
        }
    }
}
