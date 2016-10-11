using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public class GetExistingZonesRoutingProductsInput
    {
        public int SellingNumberPlanId { get; set; }
        public DateTime MinimumDate { get; set; }
    }
    public class GetExistingZonesRoutingProductsOutput
    {
        public IEnumerable<SaleZoneRoutingProduct> ExistingSaleZonesRoutingProductEntities { get; set; }
    }
    public sealed class GetExistingZonesRoutingProducts : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingZonesRoutingProductsInput, GetExistingZonesRoutingProductsOutput>
    {
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanID { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleZoneRoutingProduct>> ExistingSaleZonesRoutingProductEntities { get; set; }

        protected override GetExistingZonesRoutingProductsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingZonesRoutingProductsInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SellingNumberPlanId = this.SellingNumberPlanID.Get(context),
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleZonesRoutingProductEntities.Get(context) == null)
                this.ExistingSaleZonesRoutingProductEntities.Set(context, new List<SaleZoneRoutingProduct>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingZonesRoutingProductsOutput DoWorkWithResult(GetExistingZonesRoutingProductsInput inputArgument, AsyncActivityHandle handle)
        {

            SaleEntityServiceManager saleEntityServiceManager = new SaleEntityServiceManager();
            IEnumerable<SaleZoneRoutingProduct> saleZonesRoutingProducts = saleEntityServiceManager.GetSaleZonesRoutingProductsEffectiveAfter(inputArgument.SellingNumberPlanId, Vanrise.Common.Utilities.Min(inputArgument.MinimumDate, DateTime.Today));
            return new GetExistingZonesRoutingProductsOutput()
            {
                ExistingSaleZonesRoutingProductEntities = saleZonesRoutingProducts
            };
        }
        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingZonesRoutingProductsOutput result)
        {
            this.ExistingSaleZonesRoutingProductEntities.Set(context, result.ExistingSaleZonesRoutingProductEntities);
        }
    }
}
