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
    #region Classes

    public class ReserveIdsForNewEntitiesInput
    {
        public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }
        public IEnumerable<NewRate> OwnerNewRates { get; set; }
        public IEnumerable<NewRate> NewRatesToFillGapsDueToClosingCountry { get; set; }
        public IEnumerable<NewRate> NewRatesToFillGapsDueToChangeSellingProductRates { get; set; }
        public IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { get; set; }
        public NewDefaultService NewDefaultService { get; set; }
        public IEnumerable<NewSaleZoneService> NewSaleZoneServices { get; set; }
        public IEnumerable<NewCustomerCountry> NewCustomerCountries { get; set; }
        public Dictionary<int, List<NewPriceList>> CustomerPriceListsByCurrencyId { get; set; }
    }

    public class ReserveIdsForNewEntitiesOutput
    {
        public int? ReservedOwnerPriceListId { get; set; }
    }

    #endregion

    public class ReserveIdsForNewEntities : BaseAsyncActivity<ReserveIdsForNewEntitiesInput, ReserveIdsForNewEntitiesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }

        [RequiredArgument]
        public InArgument<NewDefaultService> NewDefaultService { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneService>> NewSaleZoneServices { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewCustomerCountry>> NewCustomerCountries { get; set; }

        [RequiredArgument]
        public OutArgument<int?> ReservedOwnerPriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> OwnerNewRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRatesToFillGapsDueToClosingCountry { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRatesToFillGapsDueToChangeSellingProductRates { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<int, List<NewPriceList>>> CustomerPriceListsByCurrencyId { get; set; }


        #endregion

        protected override ReserveIdsForNewEntitiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReserveIdsForNewEntitiesInput()
            {
                NewDefaultRoutingProduct = this.NewDefaultRoutingProduct.Get(context),
                NewSaleZoneRoutingProducts = this.NewSaleZoneRoutingProducts.Get(context),
                NewDefaultService = this.NewDefaultService.Get(context),
                NewSaleZoneServices = this.NewSaleZoneServices.Get(context),
                NewCustomerCountries = NewCustomerCountries.Get(context),
                OwnerNewRates = this.OwnerNewRates.Get(context),
                NewRatesToFillGapsDueToClosingCountry = this.NewRatesToFillGapsDueToClosingCountry.Get(context),
                NewRatesToFillGapsDueToChangeSellingProductRates = this.NewRatesToFillGapsDueToChangeSellingProductRates.Get(context),
                CustomerPriceListsByCurrencyId = this.CustomerPriceListsByCurrencyId.Get(context),
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
            IEnumerable<NewRate> ownerNewRates = inputArgument.OwnerNewRates;
            IEnumerable<NewRate> newRatesToFillGapsDueToClosingCountry = inputArgument.NewRatesToFillGapsDueToClosingCountry;
            IEnumerable<NewRate> newRatesToFillGapsDueToChangeSellingProductRates = inputArgument.NewRatesToFillGapsDueToChangeSellingProductRates;
            IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts = inputArgument.NewSaleZoneRoutingProducts;
            NewDefaultService newDefaultService = inputArgument.NewDefaultService;
            IEnumerable<NewSaleZoneService> newSaleZoneServices = inputArgument.NewSaleZoneServices;
            IEnumerable<NewCustomerCountry> newCustomerCountries = inputArgument.NewCustomerCountries;
            Dictionary<int, List<NewPriceList>> customerPriceListsByCurrencyId = inputArgument.CustomerPriceListsByCurrencyId;
            int? reservedOwnerPriceListId = null;

            RatePlanContext ratePlanContext = handle.CustomData.GetRecord("RatePlanContext") as RatePlanContext;
            int userId = handle.SharedInstanceData.InstanceInfo.InitiatorUserId;

            ReserveNewDefaultRoutingProductId(newDefaultRoutingProduct);

            List<NewRate> newRates = new List<NewRate>();
            if (ownerNewRates.Any())
            {
                reservedOwnerPriceListId = ReservePricelistId(ownerNewRates);
                newRates.AddRange(ownerNewRates);
            }

            if (newRatesToFillGapsDueToClosingCountry.Any())
            {
                ReservePricelistForNewRatesToFillGapsDueToClosingCountry(newRatesToFillGapsDueToClosingCountry, customerPriceListsByCurrencyId, ratePlanContext, userId, reservedOwnerPriceListId);
                newRates.AddRange(newRatesToFillGapsDueToClosingCountry);
            }

            if (newRatesToFillGapsDueToChangeSellingProductRates.Any())
                newRates.AddRange(newRatesToFillGapsDueToChangeSellingProductRates);

            ReserveNewRateIds(newRates);
            ReserveNewSaleZoneRoutingProductIds(newSaleZoneRoutingProducts);
            ReserveNewDefaultServiceId(newDefaultService);
            ReserveNewSaleZoneServiceIds(newSaleZoneServices);
            ReserveNewCustomerCountryIds(newCustomerCountries);

            return new ReserveIdsForNewEntitiesOutput() { ReservedOwnerPriceListId = reservedOwnerPriceListId };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ReserveIdsForNewEntitiesOutput result)
        {
            ReservedOwnerPriceListId.Set(context, result.ReservedOwnerPriceListId);
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

        private int ReservePricelistId(IEnumerable<NewRate> newRates)
        {
            int ownerPriceListId = (int)new SalePriceListManager().ReserveIdRange(1);

            foreach (NewRate newRate in newRates)
            {
                newRate.PriceListId = ownerPriceListId;
            }

            return ownerPriceListId;
        }

        private void ReservePricelistForNewRatesToFillGapsDueToClosingCountry(IEnumerable<NewRate> newRates, Dictionary<int, List<NewPriceList>> customerPriceListsByCurrencyId, RatePlanContext context, int userId, int? reservedOwnerPriceListId)
        {
            var carrierAccountManager = new CarrierAccountManager();
            var sellingProductManager = new SellingProductManager();
            var sellingProductId = carrierAccountManager.GetSellingProductId(context.OwnerId);
            var currencyId = sellingProductManager.GetSellingProductCurrencyId(sellingProductId);
            int priceListId;

            if (currencyId != context.CurrencyId || !reservedOwnerPriceListId.HasValue)
            {
                List<NewPriceList> customerPriceLists = customerPriceListsByCurrencyId.GetOrCreateItem(currencyId, () => { return new List<NewPriceList>(); });
                NewPriceList customerPriceList = customerPriceLists.FindRecord(x => x.OwnerId == context.OwnerId);
                if (customerPriceList == null)
                {
                    customerPriceList = (new NewPriceList()
                    {
                        ProcessInstanceId = context.RootProcessInstanceId,
                        UserId = userId,
                        PriceListId = (long)new SalePriceListManager().ReserveIdRange(1),
                        PriceListType = SalePriceListType.None,
                        OwnerType = SalePriceListOwnerType.Customer,
                        OwnerId = context.OwnerId,
                        CurrencyId = currencyId,
                        EffectiveOn = context.PriceListCreationDate
                    });
                    customerPriceLists.Add(customerPriceList);
                }
                priceListId = (int)customerPriceList.PriceListId;
            }
            else priceListId = reservedOwnerPriceListId.Value;
            
            foreach (NewRate newRate in newRates)
            {
                newRate.PriceListId = priceListId;
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
