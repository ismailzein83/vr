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
    public class ApplyNewDefaultRoutingProductToDBInput
    {
        public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }
    }

    public class ApplyNewDefaultRoutingProductToDBOutput
    {

    }

    public class ApplyNewDefaultRoutingProductToDB : BaseAsyncActivity<ApplyNewDefaultRoutingProductToDBInput, ApplyNewDefaultRoutingProductToDBOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }

        #endregion

        protected override ApplyNewDefaultRoutingProductToDBInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ApplyNewDefaultRoutingProductToDBInput()
            {
                NewDefaultRoutingProduct = this.NewDefaultRoutingProduct.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            base.OnBeforeExecute(context, handle);
        }

        protected override ApplyNewDefaultRoutingProductToDBOutput DoWorkWithResult(ApplyNewDefaultRoutingProductToDBInput inputArgument, AsyncActivityHandle handle)
        {
            NewDefaultRoutingProduct newDefaultRoutingProduct = inputArgument.NewDefaultRoutingProduct;

            if (newDefaultRoutingProduct != null)
            {
                var dataManager = SalesDataManagerFactory.GetDataManager<INewDefaultRoutingProductDataManager>();
                dataManager.ProcessInstanceId = handle.SharedInstanceData.InstanceInfo.ProcessInstanceID;
                dataManager.Insert(newDefaultRoutingProduct);
            }

            return new ApplyNewDefaultRoutingProductToDBOutput() { };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ApplyNewDefaultRoutingProductToDBOutput result)
        {

        }
    }
}
