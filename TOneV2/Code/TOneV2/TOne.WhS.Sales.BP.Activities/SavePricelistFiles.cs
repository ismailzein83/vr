using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class SavePricelistFiles : CodeActivity
    {
        #region Input Arguments
        public InArgument<int> CurrencyId { get; set; }
        public InArgument<int> OwnerId { get; set; }
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
        public InArgument<IEnumerable<CustomerPriceListChange>> CustomerChanges { get; set; }
        public InArgument<IEnumerable<NewCustomerPriceListChange>> NewCustomerChanges { get; set; }
        public InArgument<IEnumerable<NewPriceList>> NewSalePriceList { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SalePLZoneChange>> SalePLZoneChanges { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }

        #endregion



        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            IEnumerable<CustomerCountryToChange> countriesToChange = CustomerCountriesToChange.Get(context);
            IEnumerable<NewCustomerPriceListChange> customerPriceListChanges = NewCustomerChanges.Get(context);
            int currencyId = CurrencyId.Get(context);
            IEnumerable<NewPriceList> salePriceLists = NewSalePriceList.Get(context);

            SalePLChangeType plChangeType;

            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;

            if (countriesToChange != null && countriesToChange.Any())
            {
                plChangeType = SalePLChangeType.CountryAndRate;
            }
            else
            {
                plChangeType = SalePLChangeType.Rate;
            }
            var salePricelistFileContext = new SalePricelistFileContext
            {
                SellingNumberPlanId = ratePlanContext.OwnerSellingNumberPlanId,
                ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
                CustomerPriceListChanges = customerPriceListChanges,
                EffectiveDate = ratePlanContext.EffectiveDate,
                ChangeType = plChangeType,
                CurrencyId = currencyId,
                UserId = userId,
                SalePriceLists = salePriceLists
            };
            var salePricelistManager = new SalePriceListManager();
            salePricelistManager.SavePriceList(salePricelistFileContext);
        }

        #region Private Methods

        #endregion
    }
}
