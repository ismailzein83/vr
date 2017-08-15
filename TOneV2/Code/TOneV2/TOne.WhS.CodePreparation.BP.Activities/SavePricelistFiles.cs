using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using TOne.WhS.CodePreparation.Business;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class SavePricelistFiles : CodeActivity
    {
        public InArgument<IEnumerable<SalePLZoneChange>> ZoneChanges { get; set; }

        [RequiredArgument]
        public InArgument<SalePriceListsByOwner> SalePriceListsByOwner { get; set; }

        [RequiredArgument]
        public InArgument<IEnumerable<CustomerPriceListChange>> CustomerChanges { get; set; }
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CarrierAccountInfo>> CustomersWithPriceListFile { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);
            SalePriceListsByOwner salePriceListByOwner = SalePriceListsByOwner.Get(context);
            IEnumerable<CustomerPriceListChange> customerPriceListChanges = CustomerChanges.Get(context);
            DateTime minimumDate = MinimumDate.Get(context);
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;
            IEnumerable<SalePriceList> salePriceLists = ConvertPriceList(salePriceListByOwner, processInstanceId);
            SavePriceLists(sellingNumberPlanId, customerPriceListChanges, processInstanceId, minimumDate, salePriceLists, userId);
            var customersToSave = GetCustomersToSavePriceListsFor(sellingNumberPlanId, customerPriceListChanges);
            CustomersWithPriceListFile.Set(context, customersToSave);
        }
        private IEnumerable<SalePriceList> ConvertPriceList(SalePriceListsByOwner salePriceListsByOwner, long processInstanceId)
        {
            return salePriceListsByOwner.GetSalePriceLists().Select(priceListItem => new SalePriceList
            {
                OwnerId = priceListItem.OwnerId,
                PriceListId = priceListItem.PriceListId,
                CurrencyId = priceListItem.CurrencyId,
                OwnerType = priceListItem.OwnerType,
                PriceListType = SalePriceListType.Country,
                EffectiveOn = priceListItem.EffectiveOn,
                ProcessInstanceId = processInstanceId
            });
        }
        private void SavePriceLists(int sellingNumberPlanId, IEnumerable<CustomerPriceListChange> customerPriceListChanges, long processInstanceId, DateTime effectiveOn, IEnumerable<SalePriceList> salePriceLists, int userId)
        {
            ISalePricelistFileContext salePriceListFileContext = new SalePricelistFileContext
            {
                SellingNumberPlanId = sellingNumberPlanId,
                ProcessInstanceId = processInstanceId,
                CustomerPriceListChanges = customerPriceListChanges,
                EffectiveDate = effectiveOn,
                ChangeType = SalePLChangeType.CodeAndRate,
                SalePriceLists = salePriceLists,
                UserId = userId
            };
            SalePriceListManager salePricelistManager = new SalePriceListManager();
            salePricelistManager.SavePricelistFiles(salePriceListFileContext);
        }
        private IEnumerable<CarrierAccountInfo> GetCustomersToSavePriceListsFor(int sellingNumberPlanId, IEnumerable<CustomerPriceListChange> customerPriceListChanges)
        {
            var customersToSavePricelistsFor = new List<CarrierAccountInfo>();
            var carrierAccountManager = new CarrierAccountManager();
            var customers = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId, true).ToDictionary(c => c.CarrierAccountId, c => c);

            foreach (var customer in customerPriceListChanges)
            {
                CarrierAccountInfo accountInfo;
                if (customers.TryGetValue(customer.CustomerId, out accountInfo))
                    customersToSavePricelistsFor.Add(accountInfo);
            }
            return customersToSavePricelistsFor;
        }
    }
}

