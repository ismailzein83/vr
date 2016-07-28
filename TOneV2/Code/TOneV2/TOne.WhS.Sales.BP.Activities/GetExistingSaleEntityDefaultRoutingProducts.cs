using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleEntityDefaultRoutingProductsInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingSaleEntityDefaultRoutingProductsOutput
    {
        public IEnumerable<DefaultRoutingProduct> ExistingSaleEntityDefaultRoutingProducts { get; set; }
    }

    public class GetExistingSaleEntityDefaultRoutingProducts : BaseAsyncActivity<GetExistingSaleEntityDefaultRoutingProductsInput, GetExistingSaleEntityDefaultRoutingProductsOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<DefaultRoutingProduct>> ExistingSaleEntityDefaultRoutingProducts { get; set; }
        
        #endregion

        protected override GetExistingSaleEntityDefaultRoutingProductsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingSaleEntityDefaultRoutingProductsInput()
            {
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleEntityDefaultRoutingProducts.Get(context) == null)
                this.ExistingSaleEntityDefaultRoutingProducts.Set(context, new List<DefaultRoutingProduct>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingSaleEntityDefaultRoutingProductsOutput DoWorkWithResult(GetExistingSaleEntityDefaultRoutingProductsInput inputArgument, AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var saleEntityRoutingProductManager = new SaleEntityRoutingProductManager();
            IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = saleEntityRoutingProductManager.GetDefaultRoutingProductsEffectiveAfter(ownerType, ownerId, minimumDate);

            return new GetExistingSaleEntityDefaultRoutingProductsOutput
            {
                ExistingSaleEntityDefaultRoutingProducts = defaultRoutingProducts
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingSaleEntityDefaultRoutingProductsOutput result)
        {
            this.ExistingSaleEntityDefaultRoutingProducts.Set(context, result.ExistingSaleEntityDefaultRoutingProducts);
        }
    }
}
