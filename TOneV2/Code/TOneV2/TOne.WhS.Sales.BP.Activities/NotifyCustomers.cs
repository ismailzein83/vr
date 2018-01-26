using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using Vanrise.BusinessProcess;
using Vanrise.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.BP.Activities
{
    public class NotifyCustomers : CodeActivity
    {
        #region Input Arguments

        [RequiredArgument]
        public InArgument<IEnumerable<int>> CustomerIds { get; set; }

        #endregion

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<int> customerIds = CustomerIds.Get(context);
            int initiatorId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            long processInstanceId = context.GetRatePlanContext().RootProcessInstanceId;

            NotificationManager notificationManager = new NotificationManager();
            IEnumerable<int> failedCustomerIdsToSendEmailFor = notificationManager.SendNotification(initiatorId, customerIds, processInstanceId);
            IEnumerable<int> customersToUpdatePricelistsFor = customerIds.FindAllRecords(x => !failedCustomerIdsToSendEmailFor.Contains(x));

            if (customersToUpdatePricelistsFor != null && customersToUpdatePricelistsFor.Any())
            {
                SalePriceListManager salePriceListManager = new SalePriceListManager();
                salePriceListManager.SetCustomerPricelistsAsSent(customersToUpdatePricelistsFor, null);
            }

            if (failedCustomerIdsToSendEmailFor.Count() > 0)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                List<string> customerNames = new List<string>();
                foreach (int customerId in failedCustomerIdsToSendEmailFor)
                    customerNames.Add(carrierAccountManager.GetCarrierAccountName(customerId));

                var orderCustomersByName = customerNames.OrderBy(name => name);
                string customers = string.Join(", ", orderCustomersByName.ToArray());
                context.WriteTrackingMessage(LogEntryType.Warning, "Failed Sending Sale Pricelists to Customers: {0}.", customers);
            }
        }
    }
}
