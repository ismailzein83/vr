using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Business;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ReserveIdForPriceList : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<int> PriceListId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            TOne.WhS.BusinessEntity.Business.SupplierPriceListManager priceListManager = new TOne.WhS.BusinessEntity.Business.SupplierPriceListManager();
            int priceListId = (int)priceListManager.ReserveIDRange(1);

            PriceListId.Set(context, priceListId);
        }
    }
}
