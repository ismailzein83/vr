using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleEntityZoneRoutingProductsInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingSaleEntityZoneRoutingProductsOutput
    {
        public IEnumerable<SaleZoneRoutingProduct> ExistingSaleEntityZoneRoutingProducts { get; set; }
    }

    public class GetExistingSaleEntityZoneRoutingProducts : BaseAsyncActivity<GetExistingSaleEntityZoneRoutingProductsInput, GetExistingSaleEntityZoneRoutingProductsOutput>
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
        public OutArgument<IEnumerable<SaleZoneRoutingProduct>> ExistingSaleEntityZoneRoutingProducts { get; set; }
        
        #endregion

        protected override GetExistingSaleEntityZoneRoutingProductsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingSaleEntityZoneRoutingProductsInput()
            {
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleEntityZoneRoutingProducts.Get(context) == null)
                this.ExistingSaleEntityZoneRoutingProducts.Set(context, new List<SaleZoneRoutingProduct>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingSaleEntityZoneRoutingProductsOutput DoWorkWithResult(GetExistingSaleEntityZoneRoutingProductsInput inputArgument, AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var saleEntityRoutingProductManager = new SaleEntityRoutingProductManager();
            IEnumerable<SaleZoneRoutingProduct> saleZoneRoutingProducts = saleEntityRoutingProductManager.GetSaleZoneRoutingProductsEffectiveAfter(ownerType, ownerId, minimumDate);

            return new GetExistingSaleEntityZoneRoutingProductsOutput()
            {
                ExistingSaleEntityZoneRoutingProducts = saleZoneRoutingProducts
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingSaleEntityZoneRoutingProductsOutput result)
        {
            this.ExistingSaleEntityZoneRoutingProducts.Set(context, result.ExistingSaleEntityZoneRoutingProducts);
        }
    }
}
