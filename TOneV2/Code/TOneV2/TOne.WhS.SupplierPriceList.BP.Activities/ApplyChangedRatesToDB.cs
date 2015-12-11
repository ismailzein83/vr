using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyChangedRatesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedRate>> ChangedRates { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedRate> ratesList = this.ChangedRates.Get(context);
            int priceListId = this.PriceListId.Get(context);

            ChangedSupplierRateManager manager = new ChangedSupplierRateManager();
            manager.Insert(priceListId, ratesList);
        }
    }
}
