using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class SyncImportedDataWithDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<int> CurrencyId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int priceListId = this.PriceListId.Get(context);
            int supplierId = this.SupplierId.Get(context);
            int currencyId = this.CurrencyId.Get(context);

            TOne.WhS.SupplierPriceList.Business.SupplierPriceListManager manager = new Business.SupplierPriceListManager();
            manager.AddPriceListAndSyncImportedDataWithDB(priceListId, supplierId, currencyId);
        }
    }
}
