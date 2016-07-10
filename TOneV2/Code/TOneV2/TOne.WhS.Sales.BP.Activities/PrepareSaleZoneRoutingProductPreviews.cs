using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareSaleZoneRoutingProductPreviews : CodeActivity
    {
        #region Input Arguments
        
        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToAdd>> SaleZoneRoutingProductsToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToClose>> SaleZoneRoutingProductsToClose { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }
        
        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneRoutingProductPreview>> SaleZoneRoutingProductPreviews { get; set; }
        
        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = SaleZoneRoutingProductsToAdd.Get(context);
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = SaleZoneRoutingProductsToClose.Get(context);
            
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            DateTime minimumDate = MinimumDate.Get(context);

            var saleZoneRoutingProductPreviews = new List<SaleZoneRoutingProductPreview>();
            var routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(minimumDate));
            var routingProductManager = new RoutingProductManager();

            if (saleZoneRoutingProductsToAdd != null)
            {
                foreach (SaleZoneRoutingProductToAdd saleZoneRoutingProductToAdd in saleZoneRoutingProductsToAdd)
                {
                    var saleZoneRoutingProductPreview = new SaleZoneRoutingProductPreview()
                    {
                        ZoneName = saleZoneRoutingProductToAdd.ZoneName,
                        NewSaleZoneRoutingProductName = routingProductManager.GetRoutingProductName(saleZoneRoutingProductToAdd.ZoneRoutingProductId),
                        EffectiveOn = saleZoneRoutingProductToAdd.BED
                    };

                    SaleEntityZoneRoutingProduct currentSaleZoneRoutingProduct = null;

                    if (ownerType == SalePriceListOwnerType.SellingProduct)
                    {
                        currentSaleZoneRoutingProduct = routingProductLocator.GetSellingProductZoneRoutingProduct(ownerId, saleZoneRoutingProductToAdd.ZoneId);
                        
                        if (currentSaleZoneRoutingProduct != null)
                        {
                            saleZoneRoutingProductPreview.CurrentSaleZoneRoutingProductName = routingProductManager.GetRoutingProductName(currentSaleZoneRoutingProduct.RoutingProductId);
                            saleZoneRoutingProductPreview.IsCurrentSaleZoneRoutingProductInherited = (currentSaleZoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.ProductZone);
                        }
                    }
                    else
                    {
                        int sellingProductId = GetSellingProductId(ownerId, saleZoneRoutingProductToAdd.BED, false);
                        currentSaleZoneRoutingProduct = routingProductLocator.GetCustomerZoneRoutingProduct(ownerId, sellingProductId, saleZoneRoutingProductToAdd.ZoneId);

                        if (currentSaleZoneRoutingProduct != null)
                        {
                            saleZoneRoutingProductPreview.CurrentSaleZoneRoutingProductName = routingProductManager.GetRoutingProductName(currentSaleZoneRoutingProduct.RoutingProductId);
                            saleZoneRoutingProductPreview.IsCurrentSaleZoneRoutingProductInherited = (currentSaleZoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone);
                        }
                    }

                    saleZoneRoutingProductPreviews.Add(saleZoneRoutingProductPreview);
                }
            }

            if (saleZoneRoutingProductsToClose != null)
            {
                foreach (SaleZoneRoutingProductToClose saleZoneRoutingProductToClose in saleZoneRoutingProductsToClose)
                {
                    var saleZoneRoutingProductPreview = new SaleZoneRoutingProductPreview()
                    {
                        ZoneName = saleZoneRoutingProductToClose.ZoneName,
                        EffectiveOn = saleZoneRoutingProductToClose.CloseEffectiveDate
                    };

                    SaleEntityZoneRoutingProduct currentSaleZoneRoutingProduct = null;

                    if (ownerType == SalePriceListOwnerType.SellingProduct)
                    {
                        currentSaleZoneRoutingProduct = routingProductLocator.GetSellingProductZoneRoutingProduct(ownerId, saleZoneRoutingProductToClose.ZoneId);
                    }
                    else
                    {
                        int sellingProductId = GetSellingProductId(ownerId, saleZoneRoutingProductToClose.CloseEffectiveDate, false);
                        currentSaleZoneRoutingProduct = routingProductLocator.GetCustomerZoneRoutingProduct(ownerId, sellingProductId, saleZoneRoutingProductToClose.ZoneId);
                    }

                    if (currentSaleZoneRoutingProduct == null)
                        throw new NullReferenceException("currentSaleZoneRoutingProduct");

                    saleZoneRoutingProductPreview.CurrentSaleZoneRoutingProductName = routingProductManager.GetRoutingProductName(currentSaleZoneRoutingProduct.RoutingProductId);
                    
                    saleZoneRoutingProductPreview.IsCurrentSaleZoneRoutingProductInherited =
                        (ownerType == SalePriceListOwnerType.SellingProduct && currentSaleZoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.ProductZone) ||
                        (ownerType == SalePriceListOwnerType.Customer && currentSaleZoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone);

                    saleZoneRoutingProductPreviews.Add(saleZoneRoutingProductPreview);
                }
            }

            SaleZoneRoutingProductPreviews.Set(context, (saleZoneRoutingProductPreviews.Count > 0) ? saleZoneRoutingProductPreviews : null);
        }

        #region Private Methods

        private int GetSellingProductId(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            var customerSellingProductManager = new CustomerSellingProductManager();
            int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, isEffectiveInFuture);
            if (!sellingProductId.HasValue)
                throw new NullReferenceException("sellingProductId");
            return sellingProductId.Value;
        }
        
        #endregion
    }
}
