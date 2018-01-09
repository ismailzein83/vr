using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
	#region Classes

	public class ReserveIdsForNewEntitiesInput
	{
		public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }
		public IEnumerable<NewRate> NewRates { get; set; }
		public IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { get; set; }
		public NewDefaultService NewDefaultService { get; set; }
		public IEnumerable<NewSaleZoneService> NewSaleZoneServices { get; set; }
		public IEnumerable<NewCustomerCountry> NewCustomerCountries { get; set; }
	}

	public class ReserveIdsForNewEntitiesOutput
	{

	}
	
	#endregion

    public class ReserveIdsForNewEntities : BaseAsyncActivity<ReserveIdsForNewEntitiesInput, ReserveIdsForNewEntitiesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }

        [RequiredArgument]
        public InArgument<NewDefaultService> NewDefaultService { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneService>> NewSaleZoneServices { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<NewCustomerCountry>> NewCustomerCountries { get; set; }

        #endregion
        
        protected override ReserveIdsForNewEntitiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReserveIdsForNewEntitiesInput()
            {
                NewDefaultRoutingProduct = this.NewDefaultRoutingProduct.Get(context),
                NewRates = this.NewRates.Get(context),
                NewSaleZoneRoutingProducts = this.NewSaleZoneRoutingProducts.Get(context),
                NewDefaultService = this.NewDefaultService.Get(context),
                NewSaleZoneServices = this.NewSaleZoneServices.Get(context),
				NewCustomerCountries = NewCustomerCountries.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            handle.CustomData.Add("RatePlanContext", ratePlanContext);
            base.OnBeforeExecute(context, handle);
        }

        protected override ReserveIdsForNewEntitiesOutput DoWorkWithResult(ReserveIdsForNewEntitiesInput inputArgument, AsyncActivityHandle handle)
        {
            NewDefaultRoutingProduct newDefaultRoutingProduct = inputArgument.NewDefaultRoutingProduct;
            IEnumerable<NewRate> newRates = inputArgument.NewRates;
            IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts = inputArgument.NewSaleZoneRoutingProducts;
            NewDefaultService newDefaultService = inputArgument.NewDefaultService;
            IEnumerable<NewSaleZoneService> newSaleZoneServices = inputArgument.NewSaleZoneServices;
			IEnumerable<NewCustomerCountry> newCustomerCountries = inputArgument.NewCustomerCountries;

            IRatePlanContext ratePlanContext = handle.CustomData.GetRecord("RatePlanContext") as IRatePlanContext;

            ReserveNewDefaultRoutingProductId(newDefaultRoutingProduct);
            ReserveNewRateIds(newRates);
            if (newRates.Count() > 0)
                ReservePricelistId(newRates, ratePlanContext);
            ReserveNewSaleZoneRoutingProductIds(newSaleZoneRoutingProducts);
            ReserveNewDefaultServiceId(newDefaultService);
            ReserveNewSaleZoneServiceIds(newSaleZoneServices);
			ReserveNewCustomerCountryIds(newCustomerCountries);

            return new ReserveIdsForNewEntitiesOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ReserveIdsForNewEntitiesOutput result)
        {

        }

        #region Private Methods

        private void ReserveNewDefaultRoutingProductId(NewDefaultRoutingProduct newDefaultRoutingProduct)
        {
            if (newDefaultRoutingProduct == null)
                return;

            var ratePlanManager = new TOne.WhS.BusinessEntity.Business.SaleEntityRoutingProductManager();
            long startingId = ratePlanManager.ReserveIdRange(1);

            newDefaultRoutingProduct.SaleEntityRoutingProductId = (int)startingId;
        }

        private void ReserveNewSaleZoneRoutingProductIds(IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts)
        {
            var ratePlanManager = new TOne.WhS.BusinessEntity.Business.SaleEntityRoutingProductManager();
            long startingId = ratePlanManager.ReserveIdRange(newSaleZoneRoutingProducts.Count());

            foreach (NewSaleZoneRoutingProduct newSaleZoneRoutingProduct in newSaleZoneRoutingProducts)
                newSaleZoneRoutingProduct.SaleEntityRoutingProductId = startingId++;
        }

        private void ReserveNewRateIds(IEnumerable<NewRate> newRates)
        {
            var saleRateManager = new TOne.WhS.BusinessEntity.Business.SaleRateManager();
            long startingId = saleRateManager.ReserveIdRange(newRates.Count());

            foreach (NewRate newRate in newRates)
                newRate.RateId = startingId++;
        }

        private void ReservePricelistId(IEnumerable<NewRate> newRates, IRatePlanContext ratePlanContext)
        {
            int ownerPriceListId = (int)new SalePriceListManager().ReserveIdRange(1);
            ratePlanContext.OwnerPricelistId = ownerPriceListId;

            foreach (NewRate newRate in newRates)
            {
                if (newRate.PriceListId == 0)
                    newRate.PriceListId = ownerPriceListId;
            }
        }

        private void ReserveNewDefaultServiceId(NewDefaultService newDefaultService)
        {
            if (newDefaultService == null)
                return;

            var saleEntityServiceManager = new TOne.WhS.BusinessEntity.Business.SaleEntityServiceManager();
            long startingId = saleEntityServiceManager.ReserveIdRange(1);

            newDefaultService.SaleEntityServiceId = startingId;
        }

        private void ReserveNewSaleZoneServiceIds(IEnumerable<NewSaleZoneService> newSaleZoneServices)
        {
            if (newSaleZoneServices == null)
                return;

            var saleEntityServiceManager = new TOne.WhS.BusinessEntity.Business.SaleEntityServiceManager();
            long startingId = saleEntityServiceManager.ReserveIdRange(newSaleZoneServices.Count());

            foreach (NewSaleZoneService newSaleZoneService in newSaleZoneServices)
                newSaleZoneService.SaleEntityServiceId = startingId++;
        }

		private void ReserveNewCustomerCountryIds(IEnumerable<NewCustomerCountry> newCustomerCountries)
		{
			if (newCustomerCountries == null)
				return;

			var customerCountryManager = new TOne.WhS.BusinessEntity.Business.CustomerCountryManager();
			long startingId = customerCountryManager.ReserveIdRange(newCustomerCountries.Count());

			foreach (NewCustomerCountry newCustomerCountry in newCustomerCountries)
				newCustomerCountry.CustomerCountryId = startingId++;
		}

        #endregion
    }
}
