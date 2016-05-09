using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.CodePreparation.BP.Activities
{

    public class GetExistingRatesInput
    {
        public int sellingNumberPlanId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingRatesOutput
    {
        public IEnumerable<SaleRate> ExistingRatesEntities { get; set; }
    }


    public sealed class GetExistingRates : Vanrise.BusinessProcess.BaseAsyncActivity<GetExistingRatesInput, GetExistingRatesOutput>
    {
        [RequiredArgument]
        public InArgument<int> sellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleRate>> ExistingRateEntities { get; set; }

        protected override GetExistingRatesOutput DoWorkWithResult(GetExistingRatesInput inputArgument, Vanrise.BusinessProcess.AsyncActivityHandle handle)
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            List<SaleRate> saleRates = saleRateManager.GetSaleRatesEffectiveAfter(inputArgument.sellingNumberPlanId, inputArgument.MinimumDate);

            return new GetExistingRatesOutput()
            {
                ExistingRatesEntities = saleRates
            };
        }

        protected override GetExistingRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingRatesInput()
            {
                MinimumDate = this.MinimumDate.Get(context),
                sellingNumberPlanId = this.sellingNumberPlanId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingRatesOutput result)
        {
            this.ExistingRateEntities.Set(context, result.ExistingRatesEntities);
        }
    }
}
