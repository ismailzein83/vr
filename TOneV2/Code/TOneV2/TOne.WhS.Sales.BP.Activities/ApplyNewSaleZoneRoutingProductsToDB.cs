using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class ApplyNewSaleZoneRoutingProductsToDBInput
    {
        public IEnumerable<NewSaleZoneRoutingProduct> NewSaleZoneRoutingProducts { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyNewSaleZoneRoutingProductsToDBOutput
    {

    }

    public class ApplyNewSaleZoneRoutingProductsToDB : BaseAsyncActivity<ApplyNewSaleZoneRoutingProductsToDBInput, ApplyNewSaleZoneRoutingProductsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<NewSaleZoneRoutingProduct>> NewSaleZoneRoutingProducts { get; set; }

        #endregion

        protected override ApplyNewSaleZoneRoutingProductsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewSaleZoneRoutingProductsToDBInput()
            {
                NewSaleZoneRoutingProducts = this.NewSaleZoneRoutingProducts.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewSaleZoneRoutingProductsToDBOutput DoWorkWithResult(ApplyNewSaleZoneRoutingProductsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<NewSaleZoneRoutingProduct> newSaleZoneRoutingProducts = inputArgument.NewSaleZoneRoutingProducts;

            var dataManager = SalesDataManagerFactory.GetDataManager<INewSaleZoneRoutingProductDataManager>();
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyNewSaleZoneRoutingProductsToDB(newSaleZoneRoutingProducts);

            return new ApplyNewSaleZoneRoutingProductsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewSaleZoneRoutingProductsToDBOutput result)
        {

        }
    }
}
