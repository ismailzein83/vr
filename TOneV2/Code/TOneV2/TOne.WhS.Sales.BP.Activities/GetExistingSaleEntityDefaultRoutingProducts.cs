using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleEntityDefaultRoutingProducts : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<DefaultRoutingProduct>> ExistingSaleEntityDefaultRoutingProducts { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int ownerId = OwnerId.Get(context);
            DateTime minimumDate = MinimumDate.Get(context);

            DateTime tempDate = minimumDate.AddDays(-15);

            var dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
            IEnumerable<DefaultRoutingProduct> defaultRoutingProducts = dataManager.GetDefaultRoutingProductsEffectiveAfter(ownerType, ownerId, tempDate);

            ExistingSaleEntityDefaultRoutingProducts.Set(context, (defaultRoutingProducts != null && defaultRoutingProducts.Count() > 0) ? defaultRoutingProducts : null);
        }
    }
}
