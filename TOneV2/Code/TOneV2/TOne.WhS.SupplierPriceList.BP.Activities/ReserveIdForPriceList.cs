using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ReserveIdForPriceList : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<int> PriceListId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            TOne.WhS.SupplierPriceList.Business.SupplierPriceListManager priceListManager = new TOne.WhS.SupplierPriceList.Business.SupplierPriceListManager();
            int priceListId = (int)priceListManager.ReserveIDRange(1);

            PriceListId.Set(context, priceListId);
        }
    }
}
