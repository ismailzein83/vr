using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.SupplierPriceList.BP.Activities
{

    public sealed class GetExistingRates : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierRate>> ExistingRateEntities { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int supplierId = context.GetValue(this.SupplierId);
            DateTime minDate = context.GetValue(this.MinimumDate);

            SupplierRateManager supplierRateManager = new SupplierRateManager();
            List<SupplierRate> suppRates = supplierRateManager.GetSupplierRatesEffectiveAfter(supplierId, minDate);

            ExistingRateEntities.Set(context, suppRates);
        }
    }
}
