using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleRates : CodeActivity
    {
        #region Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleRate>> ExistingSaleRates { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            SalePriceListOwnerType ownerType = this.OwnerType.Get(context);
            int ownerId = this.OwnerId.Get(context);
            DateTime minimumDate = this.MinimumDate.Get(context);

            var saleRateManager = new SaleRateManager();
            IEnumerable<SaleRate> saleRates = saleRateManager.GetSaleRatesEffectiveAfter(ownerType, ownerId, minimumDate);

            this.ExistingSaleRates.Set(context, (saleRates.Count() > 0) ? saleRates : null);
        }
    }
}
