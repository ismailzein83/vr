using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class ProcessSaleZoneRoutingProductsInput
    {
        public IEnumerable<SaleZoneRoutingProductToAdd> SaleZoneRoutingProductsToAdd { get; set; }
        public IEnumerable<SaleZoneRoutingProductToClose> SaleZoneRoutingProductsToClose { get; set; }
        public IEnumerable<ExistingSaleZoneRoutingProduct> ExistingSaleZoneRoutingProducts { get; set; }
        public IEnumerable<ExistingZone> ExistingZones { get; set; }
        public IEnumerable<ExistingCustomerCountry> ExplicitlyChangedExistingCustomerCountries { get; set; }
    }

    public class ProcessSaleZoneRoutingProductsOutput
    {
        public IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { get; set; }

        public IEnumerable<ChangedSaleZoneRoutingProduct> ChangedSaleZoneRoutingProducts { get; set; }
    }

    #endregion

    public class ProcessSaleZoneRoutingProducts : BaseAsyncActivity<ProcessSaleZoneRoutingProductsInput, ProcessSaleZoneRoutingProductsOutput>
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

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingCustomerCountry>> ExplicitlyChangedExistingCustomerCountries { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedSaleZoneRoutingProduct>> ChangedSaleZoneRoutingProducts { get; set; }

        #endregion

        protected override ProcessSaleZoneRoutingProductsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessSaleZoneRoutingProductsInput()
            {
                SaleZoneRoutingProductsToAdd = this.SaleZoneRoutingProductsToAdd.Get(context),
                SaleZoneRoutingProductsToClose = this.SaleZoneRoutingProductsToClose.Get(context),
                ExistingSaleZoneRoutingProducts = this.ExistingSaleZoneRoutingProducts.Get(context),
                ExistingZones = this.ExistingZones.Get(context),
                ExplicitlyChangedExistingCustomerCountries = ExplicitlyChangedExistingCustomerCountries.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            handle.CustomData.Add("RatePlanContext", ratePlanContext);

            if (this.NewSaleZoneRoutingProducts.Get(context) == null)
                this.NewSaleZoneRoutingProducts.Set(context, new List<NewSaleZoneRoutingProduct>());

            if (this.ChangedSaleZoneRoutingProducts.Get(context) == null)
                this.ChangedSaleZoneRoutingProducts.Set(context, new List<ChangedSaleZoneRoutingProduct>());

            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessSaleZoneRoutingProductsOutput DoWorkWithResult(ProcessSaleZoneRoutingProductsInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = inputArgument.SaleZoneRoutingProductsToAdd;
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = inputArgument.SaleZoneRoutingProductsToClose;
            IEnumerable<ExistingSaleZoneRoutingProduct> existingSaleZoneRoutingProducts = inputArgument.ExistingSaleZoneRoutingProducts;
            IEnumerable<ExistingZone> existingZones = inputArgument.ExistingZones;
            IEnumerable<ExistingCustomerCountry> explicitlyChangedExistingCustomerCountries = inputArgument.ExplicitlyChangedExistingCustomerCountries;
            RatePlanContext ratePlanContext = handle.CustomData.GetRecord("RatePlanContext") as RatePlanContext;
            var processSaleZoneRoutingProductsContext = new ProcessSaleZoneRoutingProductsContext()
            {
                OwnerId = ratePlanContext.OwnerId,
                SaleZoneRoutingProductsToAdd = saleZoneRoutingProductsToAdd,
                SaleZoneRoutingProductsToClose = saleZoneRoutingProductsToClose,
                ExistingSaleZoneRoutingProducts = existingSaleZoneRoutingProducts,
                ExistingZones = existingZones,
                ExplicitlyChangedExistingCustomerCountries = explicitlyChangedExistingCustomerCountries
            };

            var priceListSaleZoneRoutingProductManager = new PriceListSaleZoneRoutingProductManager();
            priceListSaleZoneRoutingProductManager.ProcessZoneRoutingProducts(processSaleZoneRoutingProductsContext);

            if (DoZoneRoutingProductChangesExist(processSaleZoneRoutingProductsContext))
            {
               
                ratePlanContext.SetProcessHasChangesToTrueWithLock();
            }

            return new ProcessSaleZoneRoutingProductsOutput()
            {
                NewSaleZoneRoutingProducts = processSaleZoneRoutingProductsContext.NewSaleZoneRoutingProducts,
                ChangedSaleZoneRoutingProducts = processSaleZoneRoutingProductsContext.ChangedSaleZoneRoutingProducts
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessSaleZoneRoutingProductsOutput result)
        {
            this.NewSaleZoneRoutingProducts.Set(context, result.NewSaleZoneRoutingProducts);
            this.ChangedSaleZoneRoutingProducts.Set(context, result.ChangedSaleZoneRoutingProducts);
        }

        #region Private Methods

        private bool DoZoneRoutingProductChangesExist(ProcessSaleZoneRoutingProductsContext context)
        {
            if (context.NewSaleZoneRoutingProducts != null && context.NewSaleZoneRoutingProducts.Count() > 0)
                return true;

            if (context.ChangedSaleZoneRoutingProducts != null && context.ChangedSaleZoneRoutingProducts.Count() > 0)
                return true;

            return false;
        }

        #endregion
    }
}
