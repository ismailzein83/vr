using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class GetExistingCodes : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierCode>> ExistingCodeEntities { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int supplierId = context.GetValue(this.SupplierId);
            DateTime? minDate = context.GetValue(this.MinimumDate);

            SupplierCodeManager codeManager = new SupplierCodeManager();
            List<SupplierCode> suppCodes = codeManager.GetSupplierCodesEffectiveAfter(supplierId, minDate);

            ExistingCodeEntities.Set(context, suppCodes);
        }
    }
}
