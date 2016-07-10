using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleEntityZoneRoutingProducts : CodeActivity
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

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            DateTime minimumDate = MinimumDate.Get(context);

            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            IEnumerable<SaleZoneRoutingProduct> saleZoneRoutingProducts = dataManager.GetSaleZoneRoutingProductsEffectiveAfter(ownerType, ownerId, minimumDate);

            ExistingSaleEntityZoneRoutingProducts.Set(context, (saleZoneRoutingProducts != null && saleZoneRoutingProducts.Count() > 0) ? saleZoneRoutingProducts : null);
        }
    }
}
