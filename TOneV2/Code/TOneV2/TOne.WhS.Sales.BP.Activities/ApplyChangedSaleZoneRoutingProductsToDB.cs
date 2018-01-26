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
    public class ApplyChangedSaleZoneRoutingProductsToDBInput
    {
        public IEnumerable<ChangedSaleZoneRoutingProduct> ChangedSaleZoneRoutingProducts { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyChangedSaleZoneRoutingProductsToDBOutput
    {

    }

    public class ApplyChangedSaleZoneRoutingProductsToDB : BaseAsyncActivity<ApplyChangedSaleZoneRoutingProductsToDBInput, ApplyChangedSaleZoneRoutingProductsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedSaleZoneRoutingProduct>> ChangedSaleZoneRoutingProducts { get; set; }

        #endregion

        protected override ApplyChangedSaleZoneRoutingProductsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyChangedSaleZoneRoutingProductsToDBInput()
            {
                ChangedSaleZoneRoutingProducts = this.ChangedSaleZoneRoutingProducts.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyChangedSaleZoneRoutingProductsToDBOutput DoWorkWithResult(ApplyChangedSaleZoneRoutingProductsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ChangedSaleZoneRoutingProduct> changedSaleZoneRoutingProducts = inputArgument.ChangedSaleZoneRoutingProducts;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            var dataManager = SalesDataManagerFactory.GetDataManager<IChangedSaleZoneRoutingProductDataManager>();
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyChangedSaleZoneRoutingProductsToDB(changedSaleZoneRoutingProducts);

            return new ApplyChangedSaleZoneRoutingProductsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyChangedSaleZoneRoutingProductsToDBOutput result)
        {

        }
    }
}
