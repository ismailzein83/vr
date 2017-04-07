using System;
using Vanrise.GenericData.Business;
using Vanrise.Notification.Business;
using Vanrise.Notification.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountBalanceNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("0FC411D1-90FD-417C-BFDF-EC0C35B1A666"); } }

        public override string SearchRuntimeEditor { get { return "vr-accountbalance-notification-searcheditor"; } }

        public override string BodyRuntimeEditor { get { return "vr-accountbalance-notification-bodyeditor"; } }

        public override VRNotificationDetailEventPayload GetNotificationDetailEventPayload(IVRNotificationTypeGetNotificationEventPayloadContext context)
        {
            VRBalanceAlertEventPayload eventPayload = new VRNotificationManager().GetVRNotificationEventPayload<VRBalanceAlertEventPayload>(context.VRNotification);

            Guid BEDefinitionId = Guid.NewGuid();

            return new AccountBalanceNotificationDetailEventPayload()
            {
                BusinessEntityDescription = eventPayload.EntityId, //new BusinessEntityManager().GetEntityDescription(BEDefinitionId, eventPayload.EntityId),
                CurrentBalance = eventPayload.CurrentBalance,
                Threshold = eventPayload.Threshold
            };
        }

        public override bool IsVRNotificationMatched(IVRNotificationTypeIsMatchedContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class AccountBalanceNotificationDetailEventPayload : VRNotificationDetailEventPayload
    {
        public string BusinessEntityDescription { get; set; }

        public Decimal CurrentBalance { get; set; }

        public Decimal Threshold { get; set; }
    }
}
