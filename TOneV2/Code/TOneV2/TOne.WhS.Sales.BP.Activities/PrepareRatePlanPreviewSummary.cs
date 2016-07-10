using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class PrepareRatePlanPreviewSummary : CodeActivity
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

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<RatePlanPreviewSummary> RatePlanPreviewSummary { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<RateToChange> ratesToChange = RatesToChange.Get(context);
            IEnumerable<RateToClose> ratesToClose = RatesToClose.Get(context);

            DefaultRoutingProductToAdd defaultRoutingProductToAdd = DefaultRoutingProductToAdd.Get(context);
            DefaultRoutingProductToClose defaultRoutingProductToClose = DefaultRoutingProductToClose.Get(context);

            IEnumerable<SaleZoneRoutingProductToAdd> saleZoneRoutingProductsToAdd = SaleZoneRoutingProductsToAdd.Get(context);
            IEnumerable<SaleZoneRoutingProductToClose> saleZoneRoutingProductsToClose = SaleZoneRoutingProductsToClose.Get(context);

            var summary = new RatePlanPreviewSummary();

            SetRateProperties(summary, ratesToChange, ratesToClose);
            SetDefaultRoutingProductProperties(summary, defaultRoutingProductToAdd, defaultRoutingProductToClose);
            SetZoneRoutingProductProperties(summary, saleZoneRoutingProductsToAdd, saleZoneRoutingProductsToClose);

            RatePlanPreviewSummary.Set(context, summary);
        }

        #region Private Methods

        private void SetRateProperties(RatePlanPreviewSummary summary, IEnumerable<RateToChange> ratesToChange, IEnumerable<RateToClose> ratesToClose)
        {
            if (ratesToChange != null)
            {
                int numberOfNewRates = 0;
                int numberOfIncreasedRates = 0;
                int numberOfDecreasedRates = 0;

                foreach (RateToChange rateToChange in ratesToChange)
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

                summary.NumberOfNewRates = numberOfNewRates;
                summary.NumberOfIncreasedRates = numberOfIncreasedRates;
                summary.NumberOfDecreasedRates = numberOfDecreasedRates;
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

        #endregion
    }
}
