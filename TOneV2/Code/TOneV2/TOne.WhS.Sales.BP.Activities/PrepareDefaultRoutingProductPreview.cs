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
    #region Public Classes

    public class PrepareDefaultRoutingProductPreviewInput
    {
        public DefaultRoutingProductToAdd DefaultRoutingProductToAdd { get; set; }
        public DefaultRoutingProductToClose DefaultRoutingProductToClose { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
        public int OwnerId { get; set; }
        public DateTime MinimumDate { get; set; }
    }

    public class PrepareDefaultRoutingProductPreviewOutput
    {
        public DefaultRoutingProductPreview DefaultRoutingProductPreview { get; set; }
    }

    #endregion

    public class PrepareDefaultRoutingProductPreview : BaseAsyncActivity<PrepareDefaultRoutingProductPreviewInput, PrepareDefaultRoutingProductPreviewOutput>
    {
        #region Input Arguments

        public InArgument<DefaultRoutingProductToAdd> DefaultRoutingProductToAdd { get; set; }
        public InArgument<DefaultRoutingProductToClose> DefaultRoutingProductToClose { get; set; }
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
        public InArgument<int> OwnerId { get; set; }
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        public OutArgument<DefaultRoutingProductPreview> DefaultRoutingProductPreview { get; set; }

        #endregion

        protected override PrepareDefaultRoutingProductPreviewInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareDefaultRoutingProductPreviewInput()
            {
                DefaultRoutingProductToAdd = this.DefaultRoutingProductToAdd.Get(context),
                DefaultRoutingProductToClose = this.DefaultRoutingProductToClose.Get(context),
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            handle.CustomData.Add("RatePlanContext", ratePlanContext);

            if (this.DefaultRoutingProductPreview.Get(context) == null)
                this.DefaultRoutingProductPreview.Set(context, new DefaultRoutingProductPreview());

            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareDefaultRoutingProductPreviewOutput DoWorkWithResult(PrepareDefaultRoutingProductPreviewInput inputArgument, AsyncActivityHandle handle)
        {
            DefaultRoutingProductToAdd defaultRoutingProductToAdd = inputArgument.DefaultRoutingProductToAdd;
            DefaultRoutingProductToClose defaultRoutingProductToClose = inputArgument.DefaultRoutingProductToClose;

            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            IRatePlanContext ratePlanContext = handle.CustomData.GetRecord("RatePlanContext") as IRatePlanContext;

            var routingProductManager = new RoutingProductManager();
            SaleEntityZoneRoutingProduct currentRoutingProduct = GetCurrentDefaultRoutingProduct(ownerType, ownerId, ratePlanContext.EffectiveDate);
            var preview = new DefaultRoutingProductPreview() { };

            if (defaultRoutingProductToAdd != null)
            {
                if (currentRoutingProduct != null)
                {
                    preview.CurrentDefaultRoutingProductName = routingProductManager.GetRoutingProductName(currentRoutingProduct.RoutingProductId);
                    preview.IsCurrentDefaultRoutingProductInherited =
                        (ownerType == SalePriceListOwnerType.Customer && currentRoutingProduct.Source != SaleEntityZoneRoutingProductSource.CustomerDefault);
                }
                preview.NewDefaultRoutingProductName = routingProductManager.GetRoutingProductName(defaultRoutingProductToAdd.NewDefaultRoutingProduct.RoutingProductId);
                preview.EffectiveOn = defaultRoutingProductToAdd.BED;
            }
            else if (defaultRoutingProductToClose != null)
            {
                if (currentRoutingProduct == null)
                    throw new NullReferenceException("currentRoutingProduct");
                preview.CurrentDefaultRoutingProductName = routingProductManager.GetRoutingProductName(currentRoutingProduct.RoutingProductId);
                preview.IsCurrentDefaultRoutingProductInherited = false; // The user can't close an inherited default routing product
                preview.EffectiveOn = defaultRoutingProductToClose.CloseEffectiveDate;
            }
            else
            {
                preview = null;
            }

            return new PrepareDefaultRoutingProductPreviewOutput()
            {
                DefaultRoutingProductPreview = preview
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareDefaultRoutingProductPreviewOutput result)
        {
            this.DefaultRoutingProductPreview.Set(context, result.DefaultRoutingProductPreview);
        }

        #region Private Methods

        private SaleEntityZoneRoutingProduct GetCurrentDefaultRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn)
        {
            SaleEntityZoneRoutingProduct routingProduct = null;
            var routingProductLocator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(effectiveOn));

            switch (ownerType)
            {
                case SalePriceListOwnerType.SellingProduct:
                    routingProduct = routingProductLocator.GetSellingProductDefaultRoutingProduct(ownerId);
                    break;
                case SalePriceListOwnerType.Customer:
                    var customerSellingProductManager = new CustomerSellingProductManager();
                    int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(ownerId, effectiveOn, false);
                    if (!sellingProductId.HasValue)
                        throw new NullReferenceException("sellingProductId");
                    routingProduct = routingProductLocator.GetCustomerDefaultRoutingProduct(ownerId, sellingProductId.Value);
                    break;
            }

            return routingProduct;
        }

        #endregion
    }
}
