using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.SupplierPriceList.Business;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class ApplyChangedCodesToDB : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> PriceListId { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<ChangedCode>> ChangedCodes { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<ChangedCode> codesList = this.ChangedCodes.Get(context);
            int priceListId = this.PriceListId.Get(context);

            ChangedSupplierCodeManager manager = new ChangedSupplierCodeManager();
            manager.Insert(priceListId, codesList);
        }
    }
}
