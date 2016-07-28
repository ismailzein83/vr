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

namespace TOne.WhS.Sales.BP.Activities
{
    public class ReserveIdsForNewEntitiesInput
    {
        public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }

        public IEnumerable<NewRate> NewRates { get; set; }

        public IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { get; set; }
    }

    public class ReserveIdsForNewEntitiesOutput
    {

    }

    public class ReserveIdsForNewEntities : BaseAsyncActivity<ReserveIdsForNewEntitiesInput, ReserveIdsForNewEntitiesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }

        #endregion
        
        protected override ReserveIdsForNewEntitiesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ReserveIdsForNewEntitiesInput()
            {
                NewDefaultRoutingProduct = this.NewDefaultRoutingProduct.Get(context),
                NewRates = this.NewRates.Get(context),
                NewSaleZoneRoutingProducts = this.NewSaleZoneRoutingProducts.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ReserveIdsForNewEntitiesOutput DoWorkWithResult(ReserveIdsForNewEntitiesInput inputArgument, AsyncActivityHandle handle)
        {
            NewDefaultRoutingProduct newDefaultRoutingProduct = inputArgument.NewDefaultRoutingProduct;
            IEnumerable<NewRate> newRates = inputArgument.NewRates;
            IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts = inputArgument.NewSaleZoneRoutingProducts;

            ReserveNewDefaultRoutingProductId(newDefaultRoutingProduct);
            ReserveNewRateIds(newRates);
            ReserveNewSaleZoneRoutingProductIds(newSaleZoneRoutingProducts);

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

            var ratePlanManager = new TOne.WhS.Sales.Business.SaleEntityRoutingProductManager();
            long startingId = ratePlanManager.ReserveIdRange(1);

            newDefaultRoutingProduct.SaleEntityRoutingProductId = (int)startingId;
        }

        private void ReserveNewRateIds(IEnumerable<NewRate> newRates)
        {
            var saleRateManager = new SaleRateManager();
            long startingId = saleRateManager.ReserveIdRange(newRates.Count());

            foreach (NewRate newRate in newRates)
                newRate.RateId = startingId++;
        }

        private void ReserveNewSaleZoneRoutingProductIds(IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts)
        {
            var ratePlanManager = new TOne.WhS.Sales.Business.SaleEntityRoutingProductManager();
            long startingId = ratePlanManager.ReserveIdRange(newSaleZoneRoutingProducts.Count());

            foreach (NewSaleZoneRoutingProduct newSaleZoneRoutingProduct in newSaleZoneRoutingProducts)
                newSaleZoneRoutingProduct.SaleEntityRoutingProductId = startingId++;
        }

        #endregion
    }
}
