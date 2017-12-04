using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    #region Public Classes

    public class ProcessDefaultRoutingProductInput
    {
        public DefaultRoutingProductToAdd DefaultRoutingProductToAdd { get; set; }

        public DefaultRoutingProductToClose DefaultRoutingProductToClose { get; set; }

        public IEnumerable<ExistingDefaultRoutingProduct> ExistingDefaultRoutingProducts { get; set; }
    }

    public class ProcessDefaultRoutingProductOutput
    {
        public NewDefaultRoutingProduct NewDefaultRoutingProduct { get; set; }

        public IEnumerable<ChangedDefaultRoutingProduct> ChangedDefaultRoutingProducts { get; set; }
    }

    #endregion

    public class ProcessDefaultRoutingProduct : BaseAsyncActivity<ProcessDefaultRoutingProductInput, ProcessDefaultRoutingProductOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<DefaultRoutingProductToAdd> DefaultRoutingProductToAdd { get; set; }

        [RequiredArgument]
        public InArgument<DefaultRoutingProductToClose> DefaultRoutingProductToClose { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ExistingDefaultRoutingProduct>> ExistingDefaultRoutingProducts { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<NewDefaultRoutingProduct> NewDefaultRoutingProduct { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<ChangedDefaultRoutingProduct>> ChangedDefaultRoutingProducts { get; set; }

        #endregion

        protected override ProcessDefaultRoutingProductInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new ProcessDefaultRoutingProductInput()
            {
                DefaultRoutingProductToAdd = this.DefaultRoutingProductToAdd.Get(context),
                DefaultRoutingProductToClose = this.DefaultRoutingProductToClose.Get(context),
                ExistingDefaultRoutingProducts = this.ExistingDefaultRoutingProducts.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            handle.CustomData.Add("RatePlanContext", ratePlanContext);

            if (this.NewDefaultRoutingProduct.Get(context) == null)
                this.NewDefaultRoutingProduct.Set(context, new NewDefaultRoutingProduct());

            if (this.ChangedDefaultRoutingProducts.Get(context) == null)
                this.ChangedDefaultRoutingProducts.Set(context, new List<ChangedDefaultRoutingProduct>());

            base.OnBeforeExecute(context, handle);
        }

        protected override ProcessDefaultRoutingProductOutput DoWorkWithResult(ProcessDefaultRoutingProductInput inputArgument, AsyncActivityHandle handle)
        {
            DefaultRoutingProductToAdd defaultRoutingProductToAdd = inputArgument.DefaultRoutingProductToAdd;
            DefaultRoutingProductToClose defaultRoutingProductToClose = inputArgument.DefaultRoutingProductToClose;
            IEnumerable<ExistingDefaultRoutingProduct> existingDefaultRoutingProducts = inputArgument.ExistingDefaultRoutingProducts;
            RatePlanContext ratePlanContext = handle.CustomData.GetRecord("RatePlanContext") as RatePlanContext;
            ProcessDefaultRoutingProductContext processDefaultRoutingProductContext = new ProcessDefaultRoutingProductContext()
            {
                DefaultRoutingProductToAdd = defaultRoutingProductToAdd,
                DefaultRoutingProductToClose = defaultRoutingProductToClose,
                ExistingDefaultRoutingProducts = existingDefaultRoutingProducts
            };

            var priceListDefaultRoutingProductManager = new PriceListDefaultRoutingProductManager();
            priceListDefaultRoutingProductManager.ProcessDefaultRoutingProduct(processDefaultRoutingProductContext);

            if (DoDeafultRoutingProductChangesExist(processDefaultRoutingProductContext))
            {
                ratePlanContext.SetProcessHasChangesToTrueWithLock();
            }
            if (processDefaultRoutingProductContext.NewDefaultRoutingProduct != null && ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct && ratePlanContext.IsFirstSellingProductOffer.Value)
            {
                 processDefaultRoutingProductContext.NewDefaultRoutingProduct.BED = ratePlanContext.MinimumZoneBED;
            }
            return new ProcessDefaultRoutingProductOutput()
            {
                NewDefaultRoutingProduct = processDefaultRoutingProductContext.NewDefaultRoutingProduct,
                ChangedDefaultRoutingProducts = processDefaultRoutingProductContext.ChangedDefaultRoutingProducts
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ProcessDefaultRoutingProductOutput result)
        {
            this.NewDefaultRoutingProduct.Set(context, result.NewDefaultRoutingProduct);
            this.ChangedDefaultRoutingProducts.Set(context, result.ChangedDefaultRoutingProducts);
        }

        #region Private Methods

        private bool DoDeafultRoutingProductChangesExist(ProcessDefaultRoutingProductContext context)
        {
            if (context.NewDefaultRoutingProduct != null)
                return true;

            if (context.ChangedDefaultRoutingProducts != null && context.ChangedDefaultRoutingProducts.Count() > 0)
                return true;

            return false;
        }

        #endregion
    }
}
