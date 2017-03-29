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
        public InArgument<int?> RerservedSalePriceListId { get; set; }
        public InArgument<int> CurrencyId { get; set; }
        public InArgument<int> OwnerId { get; set; }
        public InArgument<SalePriceListOwnerType> OwnerType { get; set; }
        public InArgument<IEnumerable<CustomerPriceListChange>> CustomerChanges { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<SalePLZoneChange>> SalePLZoneChanges { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CustomerCountryToChange>> CustomerCountriesToChange { get; set; }

        #endregion

        #region Output Arguments

        [RequiredArgument]
        public OutArgument<IEnumerable<int>> CustomerIdsWithPriceList { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IRatePlanContext ratePlanContext = context.GetRatePlanContext();
            IEnumerable<CustomerCountryToChange> countriesToChange = CustomerCountriesToChange.Get(context);
            IEnumerable<CustomerPriceListChange> customerPriceListChanges = CustomerChanges.Get(context);
            IEnumerable<int> customerIdsWithPriceList;
            int currencyId = CurrencyId.Get(context);
            int ownerId = OwnerId.Get(context);
            SalePriceListOwnerType ownerType = OwnerType.Get(context);
            int? priceListId = RerservedSalePriceListId.Get(context);

            SalePLChangeType plChangeType;
            SalePriceListType salePriceListType;
            if (countriesToChange != null && countriesToChange.Any())
            {
                customerIdsWithPriceList = new List<int> { ratePlanContext.OwnerId };
                plChangeType = SalePLChangeType.CountryAndRate;
                salePriceListType = SalePriceListType.Country;
            }
            else
            {
                customerIdsWithPriceList = customerPriceListChanges.Select(c => c.CustomerId);
                plChangeType = SalePLChangeType.Rate;
                salePriceListType = SalePriceListType.RateChange;
            }
            var salePricelistFileContext = new SalePricelistFileContext
            {
                SellingNumberPlanId = ratePlanContext.OwnerSellingNumberPlanId,
                ProcessInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID,
                CustomerPriceListChanges = customerPriceListChanges,
                EffectiveDate = ratePlanContext.EffectiveDate,
                ChangeType = plChangeType,
                CurrencyId = currencyId
            };
            salePricelistFileContext.SalePriceLists = CreatePriceList(ownerId, ownerType, priceListId, currencyId,
                salePriceListType, salePricelistFileContext.ProcessInstanceId);
            var salePricelistManager = new SalePriceListManager();
            salePricelistManager.SavePricelistFiles(salePricelistFileContext);
            CustomerIdsWithPriceList.Set(context, customerIdsWithPriceList);
        }

        #region Private Methods

       private IEnumerable<SalePriceList> CreatePriceList(int ownerId, SalePriceListOwnerType ownerType, int? reservedId, int currencyId, SalePriceListType salePriceListType, long processInstanceId)
        {
            if (!reservedId.HasValue) return new List<SalePriceList>();
            return new List<SalePriceList>
            {
                new SalePriceList
                {
                    OwnerId = ownerId,
                    PriceListId = reservedId.Value,
                    CurrencyId = currencyId,
                    OwnerType = ownerType,
                    PriceListType = salePriceListType,
                    EffectiveOn = DateTime.Now,
                    CreatedTime = DateTime.Now,
                    ProcessInstanceId = processInstanceId
                }
            };
        }
        #endregion
    }
}
