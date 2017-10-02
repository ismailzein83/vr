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
        public InArgument<IEnumerable<NewCustomerPriceListChange>> CustomerPriceListChanges { get; set; }
        [RequiredArgument]
        public InArgument<int> SellingNumberPlanId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> MinimumDate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<CarrierAccountInfo>> CustomersWithPriceListFile { get; set; }

        public InArgument<IEnumerable<NewPriceList>> NewSalePriceList { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            int userId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            int sellingNumberPlanId = SellingNumberPlanId.Get(context);

            IEnumerable<NewCustomerPriceListChange> customerPriceListChanges = CustomerPriceListChanges.Get(context);
            IEnumerable<NewPriceList> salePriceLists = NewSalePriceList.Get(context);
            DateTime minimumDate = MinimumDate.Get(context);
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            SavePriceLists(sellingNumberPlanId, customerPriceListChanges, processInstanceId, minimumDate, salePriceLists, userId, context);
            var customersToSave = GetCustomersToSavePriceListsFor(sellingNumberPlanId, customerPriceListChanges);
            CustomersWithPriceListFile.Set(context, customersToSave);
        }

        private void SavePriceLists(int sellingNumberPlanId, IEnumerable<NewCustomerPriceListChange> customerPriceListChanges, long processInstanceId, DateTime effectiveOn, IEnumerable<NewPriceList> newSalePriceLists, int userId
            , CodeActivityContext activityContext)
        {
            ISalePricelistFileContext salePriceListFileContext = new SalePricelistFileContext
            {
                SellingNumberPlanId = sellingNumberPlanId,
                ProcessInstanceId = processInstanceId,
                CustomerPriceListChanges = customerPriceListChanges,
                EffectiveDate = effectiveOn,
                ChangeType = SalePLChangeType.CodeAndRate,
                SalePriceLists = newSalePriceLists,
                UserId = userId
            };
            SalePriceListManager salePricelistManager = new SalePriceListManager();
            salePricelistManager.SavePriceList(salePriceListFileContext);
        }
        private IEnumerable<CarrierAccountInfo> GetCustomersToSavePriceListsFor(int sellingNumberPlanId, IEnumerable<NewCustomerPriceListChange> customerPriceListChanges)
        {
            var customersToSavePricelistsFor = new List<CarrierAccountInfo>();
            if (customerPriceListChanges != null)
            {
                var carrierAccountManager = new CarrierAccountManager();
                var customers = carrierAccountManager.GetCustomersBySellingNumberPlanId(sellingNumberPlanId, true).ToDictionary(c => c.CarrierAccountId, c => c);

                foreach (var customer in customerPriceListChanges)
                {
                    CarrierAccountInfo accountInfo;
                    if (customers.TryGetValue(customer.CustomerId, out accountInfo))
                        customersToSavePricelistsFor.Add(accountInfo);
                }
            }
            return customersToSavePricelistsFor;
        }
    }
}

