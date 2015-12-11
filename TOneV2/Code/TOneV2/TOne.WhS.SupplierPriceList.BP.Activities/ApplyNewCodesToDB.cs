using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyNewCodesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<NewCode>> NewCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<NewCode> codesList = this.NewCodes.Get(context);
            int priceListId = this.PriceListId.Get(context);

            NewSupplierCodeManager manager = new NewSupplierCodeManager();
            manager.Insert(priceListId, codesList);
        }
    }
}
