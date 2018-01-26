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
    public class ApplyChangedDefaultRoutingProductsToDBInput
    {
        public IEnumerable<ChangedDefaultRoutingProduct> ChangedDefaultRoutingProducts { get; set; }
        public long RootProcessInstanceId { get; set; }
    }

    public class ApplyChangedDefaultRoutingProductsToDBOutput
    {

    }

    public class ApplyChangedDefaultRoutingProductsToDB : BaseAsyncActivity<ApplyChangedDefaultRoutingProductsToDBInput, ApplyChangedDefaultRoutingProductsToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedDefaultRoutingProduct>> ChangedDefaultRoutingProducts { get; set; }

        #endregion

        protected override ApplyChangedDefaultRoutingProductsToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyChangedDefaultRoutingProductsToDBInput()
            {
                ChangedDefaultRoutingProducts = this.ChangedDefaultRoutingProducts.Get(context),
                RootProcessInstanceId = context.GetRatePlanContext().RootProcessInstanceId,
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyChangedDefaultRoutingProductsToDBOutput DoWorkWithResult(ApplyChangedDefaultRoutingProductsToDBInput inputArgument, AsyncActivityHandle handle)
        {
            IEnumerable<ChangedDefaultRoutingProduct> changedDefaultRoutingProducts = inputArgument.ChangedDefaultRoutingProducts;
            long rootProcessInstanceId = inputArgument.RootProcessInstanceId;
            var dataManager = SalesDataManagerFactory.GetDataManager<IChangedDefaultRoutingProductDataManager>();
            dataManager.ProcessInstanceId = rootProcessInstanceId;
            dataManager.ApplyChangedDefaultRoutingProductsToDB(changedDefaultRoutingProducts);

            return new ApplyChangedDefaultRoutingProductsToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyChangedDefaultRoutingProductsToDBOutput result)
        {

        }
    }
}
