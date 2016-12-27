using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.CodePreparation.Business;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.Common;
using Vanrise.BusinessProcess;
using Vanrise.Entities;

namespace TOne.WhS.CodePreparation.BP.Activities
{
    public sealed class SendSalePLNotifications : CodeActivity
    {
        [RequiredArgument]
        public InArgument<IEnumerable<int>> CustomerIds { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IEnumerable<int> customerIds = this.CustomerIds.Get(context);
            int initiatorId = context.GetSharedInstanceData().InstanceInfo.InitiatorUserId;
            long processInstanceId = context.GetSharedInstanceData().InstanceInfo.ProcessInstanceID;

            NotificationManager notificationManager = new NotificationManager();
            IEnumerable<int> failedCustomerIdsToSendEmailFor = notificationManager.SendNotification(initiatorId, customerIds, processInstanceId);

            if (failedCustomerIdsToSendEmailFor.Count() > 0)
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                List<string> customerNames = new List<string>();
                foreach (int customerId in failedCustomerIdsToSendEmailFor)
                    customerNames.Add(carrierAccountManager.GetCarrierAccountName(customerId));

                string customers = string.Join(", ", customerNames.ToArray());
                context.WriteTrackingMessage(LogEntryType.Warning, "Failed Sending Sale Pricelists to Customers: {0}.", customers);
            }
        }

    }
}
