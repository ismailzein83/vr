using System;
using Vanrise.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.Business
{
    public class TOneAccountBalanceNotificationTypeSettings : AccountBalanceNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("EE2C731E-A5F7-481E-B350-7771C8A0F3BC"); } } 

        public override string NotificationQueryEditor { get { return "whs-accountbalance-notificationtype-searcheditor"; } }

        public override bool IsVRNotificationMatched(IAccountBalanceNotificationTypeIsMatchedContext context)
        {
            if (context.AccountBalanceNotificationExtendedQuery == null)
                return true;

            var extendedQuery = context.AccountBalanceNotificationExtendedQuery as TOneAccountBalanceNotificationExtendedQuery;
            if (extendedQuery == null)
                return false;

            if (extendedQuery.AccountTypeId.HasValue && extendedQuery.AccountTypeId.Value != context.AccountTypeId)
                return false;

            if (extendedQuery.FinancialAccountIds != null && !extendedQuery.FinancialAccountIds.Contains(context.EventPayload.EntityId))
                return false;

            return true;
        }
    }
}
