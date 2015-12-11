using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyNewRatesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewRate>> NewRates { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewRate> ratesList = this.NewRates.Get(context);
            int priceListId = this.PriceListId.Get(context);

            NewSupplierRateManager manager = new NewSupplierRateManager();
            manager.Insert(priceListId, ratesList);
        }
    }
}
