using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Sales.BP.Activities
{
    public class GetExistingSaleRatesInput
    {
        public SalePriceListOwnerType OwnerType { get; set; }

        public int OwnerId { get; set; }

        public DateTime MinimumDate { get; set; }
    }

    public class GetExistingSaleRatesOutput
    {
        public IEnumerable<SaleRate> ExistingSaleRates { get; set; }
    }

    public class GetExistingSaleRates : BaseAsyncActivity<GetExistingSaleRatesInput, GetExistingSaleRatesOutput>
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }

        [RequiredArgument]
        public InArgument<int> OwnerId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<SaleRate>> ExistingSaleRates { get; set; }
        
        #endregion

        protected override GetExistingSaleRatesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetExistingSaleRatesInput()
            {
                OwnerType = this.OwnerType.Get(context),
                OwnerId = this.OwnerId.Get(context),
                MinimumDate = this.MinimumDate.Get(context)
            };
        }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.ExistingSaleRates.Get(context) == null)
                this.ExistingSaleRates.Set(context, new List<SaleRate>());
            base.OnBeforeExecute(context, handle);
        }

        protected override GetExistingSaleRatesOutput DoWorkWithResult(GetExistingSaleRatesInput inputArgument, AsyncActivityHandle handle)
        {
            SalePriceListOwnerType ownerType = inputArgument.OwnerType;
            int ownerId = inputArgument.OwnerId;
            DateTime minimumDate = inputArgument.MinimumDate;

            var saleRateManager = new SaleRateManager();
            IEnumerable<SaleRate> saleRates = saleRateManager.GetSaleRatesEffectiveAfter(ownerType, ownerId, minimumDate);

            return new GetExistingSaleRatesOutput()
            {
                ExistingSaleRates = saleRates
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetExistingSaleRatesOutput result)
        {
            this.ExistingSaleRates.Set(context, result.ExistingSaleRates);
        }
    }
}
