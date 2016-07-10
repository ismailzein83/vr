using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ReserveIdsForNewEntities : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            NewDefaultRoutingProduct newDefaultRoutingProduct = NewDefaultRoutingProduct.Get(context);
            IEnumerable<NewRate> newRates = NewRates.Get(context);
            IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts = NewSaleZoneRoutingProducts.Get(context);

            ReserveNewDefaultRoutingProductId(newDefaultRoutingProduct);
            ReserveNewRateIds(newRates);
            ReserveNewSaleZoneRoutingProductIds(newSaleZoneRoutingProducts);
        }

        #region Private Methods

        private void ReserveNewDefaultRoutingProductId(NewDefaultRoutingProduct newDefaultRoutingProduct)
        {
            if (newDefaultRoutingProduct == null)
                return;

            var ratePlanManager = new RatePlanManager();
            long startingId = ratePlanManager.ReserveSaleEntityRoutingProductIdRange(1);

            newDefaultRoutingProduct.SaleEntityRoutingProductId = (int)startingId;
        }

        private void ReserveNewRateIds(IEnumerable<NewRate> newRates)
        {
            if (newRates == null)
                return;

            int newRatesCount = newRates.Count();

            if (newRatesCount == 0)
                return;

            var saleRateManager = new SaleRateManager();
            long startingId = saleRateManager.ReserveIdRange(newRatesCount);

            foreach (NewRate newRate in newRates)
                newRate.RateId = startingId++;
        }

        private void ReserveNewSaleZoneRoutingProductIds(IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts)
        {
            if (newSaleZoneRoutingProducts == null)
                return;

            var ratePlanManager = new RatePlanManager();
            long startingId = ratePlanManager.ReserveSaleEntityRoutingProductIdRange(newSaleZoneRoutingProducts.Count());

            foreach (NewSaleZoneRoutingProduct newSaleZoneRoutingProduct in newSaleZoneRoutingProducts)
                newSaleZoneRoutingProduct.SaleEntityRoutingProductId = startingId++;
        }

        #endregion
    }
}
