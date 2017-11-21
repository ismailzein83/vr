using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareSaleZoneRoutingProductPreviewsInput
    {
        public IEnumerable<SaleZoneRoutingProductToAdd> SaleZoneRoutingProductsToAdd { get; set; }

        public IEnumerable<SaleZoneRoutingProductToClose> SaleZoneRoutingProductsToClose { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class PrepareSaleZoneRoutingProductPreviewsOutput
    {
        public IEnumerable<SaleZoneRoutingProductPreview> SaleZoneRoutingProductPreviews { get; set; }
    }

    public class PrepareSaleZoneRoutingProductPreviews : BaseAsyncActivity<PrepareSaleZoneRoutingProductPreviewsInput, PrepareSaleZoneRoutingProductPreviewsOutput>
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

        protected override PrepareSaleZoneRoutingProductPreviewsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareSaleZoneRoutingProductPreviewsInput()
            {
                SaleZoneRoutingProductsToAdd = this.SaleZoneRoutingProductsToAdd.Get(context),
                SaleZoneRoutingProductsToClose = this.SaleZoneRoutingProductsToClose.Get(context),
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            handle.CustomData.Add("RatePlanContext", ratePlanContext);

            if (this.SaleZoneRoutingProductPreviews.Get(context) == null)
                this.SaleZoneRoutingProductPreviews.Set(context, new List<SaleZoneRoutingProductPreview>());

            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareSaleZoneRoutingProductPreviewsOutput DoWorkWithResult(PrepareSaleZoneRoutingProductPreviewsInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = inputArgument.SaleZoneRoutingProductsToAdd;
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = inputArgument.SaleZoneRoutingProductsToClose;

            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            IRatePlanContext ratePlanContext = handle.CustomData.GetRecord("RatePlanContext") as IRatePlanContext;

            var saleZoneRoutingProductPreviews = new List<SaleZoneRoutingProductPreview>();
            var routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(ratePlanContext.EffectiveDate));
            var routingProductManager = new RoutingProductManager();

            foreach (SaleZoneRoutingProductToAdd saleZoneRoutingProductToAdd in saleZoneRoutingProductsToAdd)
            {
                var saleZoneRoutingProductPreview = new SaleZoneRoutingProductPreview()
                {
                    ZoneId = saleZoneRoutingProductToAdd.ZoneId,
                    ZoneName = saleZoneRoutingProductToAdd.ZoneName,
                    NewSaleZoneRoutingProductName = routingProductManager.GetRoutingProductName(saleZoneRoutingProductToAdd.ZoneRoutingProductId),
                    NewSaleZoneRoutingProductId = saleZoneRoutingProductToAdd.ZoneRoutingProductId,
                    EffectiveOn = saleZoneRoutingProductToAdd.BED
                };

                SaleEntityZoneRoutingProduct currentSaleZoneRoutingProduct = null;

                if (ownerType == SalePriceListOwnerType.SellingProduct)
                {
                    currentSaleZoneRoutingProduct = routingProductLocator.GetSellingProductZoneRoutingProduct(ownerId, saleZoneRoutingProductToAdd.ZoneId);

                    if (currentSaleZoneRoutingProduct != null)
                    {
                        saleZoneRoutingProductPreview.CurrentSaleZoneRoutingProductName = routingProductManager.GetRoutingProductName(currentSaleZoneRoutingProduct.RoutingProductId);
                        saleZoneRoutingProductPreview.CurrentSaleZoneRoutingProductId = currentSaleZoneRoutingProduct.RoutingProductId;
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
                        saleZoneRoutingProductPreview.CurrentSaleZoneRoutingProductId = currentSaleZoneRoutingProduct.RoutingProductId;
                        saleZoneRoutingProductPreview.IsCurrentSaleZoneRoutingProductInherited = (currentSaleZoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone);
                    }
                }

                saleZoneRoutingProductPreviews.Add(saleZoneRoutingProductPreview);
            }

            if (saleZoneRoutingProductsToClose != null)
            {
                foreach (SaleZoneRoutingProductToClose saleZoneRoutingProductToClose in saleZoneRoutingProductsToClose)
                {
                    var saleZoneRoutingProductPreview = new SaleZoneRoutingProductPreview()
                    {
                        ZoneId = saleZoneRoutingProductToClose.ZoneId,
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
                    currentSaleZoneRoutingProduct.RoutingProductId = currentSaleZoneRoutingProduct.RoutingProductId;
                    saleZoneRoutingProductPreview.IsCurrentSaleZoneRoutingProductInherited =
                        (ownerType == SalePriceListOwnerType.SellingProduct && currentSaleZoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.ProductZone) ||
                        (ownerType == SalePriceListOwnerType.Customer && currentSaleZoneRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerZone);

                    saleZoneRoutingProductPreviews.Add(saleZoneRoutingProductPreview);
                }
            }

            return new PrepareSaleZoneRoutingProductPreviewsOutput()
            {
                SaleZoneRoutingProductPreviews = saleZoneRoutingProductPreviews
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareSaleZoneRoutingProductPreviewsOutput result)
        {
            this.SaleZoneRoutingProductPreviews.Set(context, result.SaleZoneRoutingProductPreviews);
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
