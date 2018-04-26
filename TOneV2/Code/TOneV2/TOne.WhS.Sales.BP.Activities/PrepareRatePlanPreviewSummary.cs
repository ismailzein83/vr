using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Classes

    public class PrepareRatePlanPreviewSummaryInput
    {
        public IEnumerable<RateToChange> RatesToChange { get; set; }

        public IEnumerable<RateToClose> RatesToClose { get; set; }

        public DefaultRoutingProductToAdd DefaultRoutingProductToAdd { get; set; }

        public DefaultRoutingProductToClose DefaultRoutingProductToClose { get; set; }

        public IEnumerable<SaleZoneRoutingProductToAdd> SaleZoneRoutingProductsToAdd { get; set; }

        public IEnumerable<SaleZoneRoutingProductToClose> SaleZoneRoutingProductsToClose { get; set; }

        public DefaultServiceToAdd DefaultServiceToAdd { get; set; }

        public DefaultServiceToClose DefaultServiceToClose { get; set; }

        public IEnumerable<SaleZoneServiceToAdd> SaleZoneServicesToAdd { get; set; }

        public IEnumerable<SaleZoneServiceToClose> SaleZoneServicesToClose { get; set; }

		public IEnumerable<CustomerCountryToChange> CustomerCountriesToChange { get; set; }
        		
        public IEnumerable<CustomerCountryToAdd> CustomerCountriesToAdd { get; set; }
    }

    public class PrepareRatePlanPreviewSummaryOutput
    {
        public RatePlanPreviewSummary RatePlanPreviewSummary { get; set; }
    }
    
    #endregion

    public class PrepareRatePlanPreviewSummary : BaseAsyncActivity<PrepareRatePlanPreviewSummaryInput, PrepareRatePlanPreviewSummaryOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<RateToChange>> RatesToChange { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<RateToClose>> RatesToClose { get; set; }

        [RequiredArgument]
        public InArgument<DefaultRoutingProductToAdd> DefaultRoutingProductToAdd { get; set; }

        [RequiredArgument]
        public InArgument<DefaultRoutingProductToClose> DefaultRoutingProductToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToAdd>> SaleZoneRoutingProductsToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneRoutingProductToClose>> SaleZoneRoutingProductsToClose { get; set; }

        [RequiredArgument]
        public InArgument<DefaultServiceToAdd> DefaultServiceToAdd { get; set; }

        [RequiredArgument]
        public InArgument<DefaultServiceToClose> DefaultServiceToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToAdd>> SaleZoneServicesToAdd { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SaleZoneServiceToClose>> SaleZoneServicesToClose { get; set; }

		[RequiredArgument]
		public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }	
        [RequiredArgument]		
        public InArgument<IEnumerable<CustomerCountryToAdd>> CustomerCountriesToAdd { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<RatePlanPreviewSummary> RatePlanPreviewSummary { get; set; }

        #endregion

        protected override PrepareRatePlanPreviewSummaryInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new PrepareRatePlanPreviewSummaryInput()
            {
                RatesToChange = this.RatesToChange.Get(context),
                RatesToClose = this.RatesToClose.Get(context),
                DefaultRoutingProductToAdd = this.DefaultRoutingProductToAdd.Get(context),
                DefaultRoutingProductToClose = this.DefaultRoutingProductToClose.Get(context),
                SaleZoneRoutingProductsToAdd = this.SaleZoneRoutingProductsToAdd.Get(context),
                SaleZoneRoutingProductsToClose = this.SaleZoneRoutingProductsToClose.Get(context),
                DefaultServiceToAdd = this.DefaultServiceToAdd.Get(context),
                DefaultServiceToClose = this.DefaultServiceToClose.Get(context),
                SaleZoneServicesToAdd = this.SaleZoneServicesToAdd.Get(context),
                SaleZoneServicesToClose = this.SaleZoneServicesToClose.Get(context),
				CustomerCountriesToChange = this.CustomerCountriesToChange.Get(context),		
                CustomerCountriesToAdd = this.CustomerCountriesToAdd.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.RatePlanPreviewSummary.Get(context) == null)
                this.RatePlanPreviewSummary.Set(context, new RatePlanPreviewSummary());
            base.OnBeforeExecute(context, handle);
        }

        protected override PrepareRatePlanPreviewSummaryOutput DoWorkWithResult(PrepareRatePlanPreviewSummaryInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<RateToChange> ratesToChange = inputArgument.RatesToChange;
            IEnumerable<RateToClose> ratesToClose = inputArgument.RatesToClose;

            DefaultRoutingProductToAdd defaultRoutingProductToAdd = inputArgument.DefaultRoutingProductToAdd;
            DefaultRoutingProductToClose defaultRoutingProductToClose = inputArgument.DefaultRoutingProductToClose;

            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = inputArgument.SaleZoneRoutingProductsToAdd;
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = inputArgument.SaleZoneRoutingProductsToClose;

            DefaultServiceToAdd defaultServiceToAdd = inputArgument.DefaultServiceToAdd;
            DefaultServiceToClose defaultServiceToClose = inputArgument.DefaultServiceToClose;

            IEnumerable<SaleZoneServiceToAdd> saleZoneServicesToAdd = inputArgument.SaleZoneServicesToAdd;
            IEnumerable<SaleZoneServiceToClose> saleZoneServicesToClose = inputArgument.SaleZoneServicesToClose;

			IEnumerable<CustomerCountryToChange> countriesToChange = inputArgument.CustomerCountriesToChange;
            IEnumerable<CustomerCountryToAdd> countriesToAdd = inputArgument.CustomerCountriesToAdd;

            var summary = new RatePlanPreviewSummary();

            SetRateProperties(summary, ratesToChange, ratesToClose);
            SetDefaultRoutingProductProperties(summary, defaultRoutingProductToAdd, defaultRoutingProductToClose);
            SetZoneRoutingProductProperties(summary, saleZoneRoutingProductsToAdd, saleZoneRoutingProductsToClose);
            SetDefaultServiceProperties(summary, defaultServiceToAdd, defaultServiceToClose);
            SetSaleZoneServiceProperties(summary, saleZoneServicesToAdd, saleZoneServicesToClose);
            SetCountryProperties(summary, countriesToChange, countriesToAdd);

            return new PrepareRatePlanPreviewSummaryOutput()
            {
                RatePlanPreviewSummary = summary
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, PrepareRatePlanPreviewSummaryOutput result)
        {
            this.RatePlanPreviewSummary.Set(context, result.RatePlanPreviewSummary);
        }

        #region Private Methods

        private void SetRateProperties(RatePlanPreviewSummary summary, IEnumerable<RateToChange> ratesToChange, IEnumerable<RateToClose> ratesToClose)
        {
            if (ratesToChange != null)
            {
                int numberOfNewRates = 0;
                int numberOfIncreasedRates = 0;
                int numberOfDecreasedRates = 0;

                int numberOfNewOtherRates = 0;
                int numberOfIncreasedOtherRates = 0;
                int numberOfDecreasedOtherRates = 0;

                foreach (RateToChange rateToChange in ratesToChange)
                {
                    if (rateToChange.RateTypeId == null)
                    {
                        switch (rateToChange.ChangeType)
                        {
                            case BusinessEntity.Entities.RateChangeType.New:
                                numberOfNewRates++;
                                break;
                            case BusinessEntity.Entities.RateChangeType.Increase:
                                numberOfIncreasedRates++;
                                break;
                            case BusinessEntity.Entities.RateChangeType.Decrease:
                                numberOfDecreasedRates++;
                                break;
                        }
                    }
                    else
                    {
                        switch (rateToChange.ChangeType)
                        {
                            case BusinessEntity.Entities.RateChangeType.New:
                                numberOfNewOtherRates++;
                                break;
                            case BusinessEntity.Entities.RateChangeType.Increase:
                                numberOfIncreasedOtherRates++;
                                break;
                            case BusinessEntity.Entities.RateChangeType.Decrease:
                                numberOfDecreasedOtherRates++;
                                break;
                        }
                    }
                }

                summary.NumberOfNewRates = numberOfNewRates;
                summary.NumberOfIncreasedRates = numberOfIncreasedRates;
                summary.NumberOfDecreasedRates = numberOfDecreasedRates;
                summary.NumberOfNewOtherRates = numberOfNewOtherRates;
                summary.NumberOfIncreasedOtherRates = numberOfIncreasedOtherRates;
                summary.NumberOfDecreasedOtherRates = numberOfDecreasedOtherRates;
            }

            if (ratesToClose != null)
            {
                summary.NumberOfClosedRates = ratesToClose.Count();
            }
        }

        private void SetDefaultRoutingProductProperties(RatePlanPreviewSummary summary, DefaultRoutingProductToAdd defaultRoutingProductToAdd, DefaultRoutingProductToClose defaultRoutingProductToClose)
        {
            var routingProductManager = new RoutingProductManager();

            if (defaultRoutingProductToAdd != null)
            {
                summary.NameOfNewDefaultRoutingProduct = routingProductManager.GetRoutingProductName(defaultRoutingProductToAdd.NewDefaultRoutingProduct.RoutingProductId);
            }
            else if (defaultRoutingProductToClose != null)
            {
                // I don't think that the below logic is valid, but I couldn't find another solution. How can I get the RoutingProductId of the default routing product to be closed?

                if (defaultRoutingProductToClose.ChangedExistingDefaultRoutingProducts == null)
                    throw new NullReferenceException("defaultRoutingProductToClose.ChangedExistingDefaultRoutingProducts");

                if (defaultRoutingProductToClose.ChangedExistingDefaultRoutingProducts.Count > 0)
                {
                    ExistingDefaultRoutingProduct existingDefaultRoutingProduct =
                    defaultRoutingProductToClose.ChangedExistingDefaultRoutingProducts.OrderByDescending(x => x.EED).FirstOrDefault();

                    summary.NameOfClosedDefaultRoutingProduct = routingProductManager.GetRoutingProductName(existingDefaultRoutingProduct.DefaultRoutingProductEntity.RoutingProductId);
                }
            }
        }

        private void SetZoneRoutingProductProperties(RatePlanPreviewSummary summary, IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd, IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose)
        {
            if (saleZoneRoutingProductsToAdd != null)
            {
                summary.NumberOfNewSaleZoneRoutingProducts = saleZoneRoutingProductsToAdd.Count();
            }

            if (saleZoneRoutingProductsToClose != null)
            {
                summary.NumberOfClosedSaleZoneRoutingProducts = saleZoneRoutingProductsToClose.Count();
            }
        }

        private void SetDefaultServiceProperties(RatePlanPreviewSummary summary, DefaultServiceToAdd defaultServiceToAdd, DefaultServiceToClose defaultServiceToClose)
        {
            if (defaultServiceToAdd != null)
                summary.NewDefaultServices = defaultServiceToAdd.NewDefaultService.Services;

            else if (defaultServiceToClose != null)
                summary.ClosedDefaultServiceEffectiveOn = defaultServiceToClose.CloseEffectiveDate;
        }

        private void SetSaleZoneServiceProperties(RatePlanPreviewSummary summary, IEnumerable<SaleZoneServiceToAdd> saleZoneServiceToAdd, IEnumerable<SaleZoneServiceToClose> closedSaleZoneServices)
        {
            summary.NumberOfNewSaleZoneServices = saleZoneServiceToAdd.Count();
            summary.NumberOfClosedSaleZoneServices = closedSaleZoneServices.Count();
        }

        private void SetCountryProperties(RatePlanPreviewSummary summary, IEnumerable<CustomerCountryToChange> customerCountriesToChange, IEnumerable<CustomerCountryToAdd> customerCountriesToAdd)
		{
            if (customerCountriesToChange != null)
            {
                summary.NumberOfChangedCountries = customerCountriesToChange.Count();
            }
            if (customerCountriesToAdd != null)
            {
                summary.NumberOfNewCountries = customerCountriesToAdd.Count();
            }
		}

        #endregion
    }
}
