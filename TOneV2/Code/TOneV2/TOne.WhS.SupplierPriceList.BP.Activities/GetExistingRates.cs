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

    public class GetExistingRatesInput
    {
        public int SupplierId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingRatesOutput
    {
        public IEnumerable<SupplierRate> ExistingRatesEntities { get; set; }
    }


    public sealed class GetExistingRates : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingRatesInput, GetExistingRatesOutput>
    {
        [RequiredArgument]
        public InArgument<int> SupplierId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SupplierRate>> ExistingRateEntities { get; set; }

        protected override GetExistingRatesOutput DoWorkWithResult(GetExistingRatesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SupplierRateManager supplierRateManager = new SupplierRateManager();
            List<SupplierRate> suppRates = supplierRateManager.GetSupplierRatesEffectiveAfter(inputArgument.SupplierId, inputArgument.MinimumDate);

            return new GetExistingRatesOutput()
            {
                ExistingRatesEntities = suppRates
            };
        }

        protected override GetExistingRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingRatesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                SupplierId = this.SupplierId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingRatesOutput result)
        {
            this.ExistingRateEntities.Set(context, result.ExistingRatesEntities);
        }
    }
}
