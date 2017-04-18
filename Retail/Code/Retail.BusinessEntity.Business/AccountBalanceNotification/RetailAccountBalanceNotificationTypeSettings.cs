using System;
using Vanrise.AccountBalance.Business;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountBalanceNotificationTypeSettings : AccountBalanceNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("52525041-7A8B-4AE1-9599-A3F34A87CB38"); } }

        public override string NotificationQueryEditor { get { return "retail-be-accountbalancenotificationtype-searcheditor"; } }

        public Guid AccountBEDefinitionId { get; set; }

        public override bool IsVRNotificationMatched(IAccountBalanceNotificationTypeIsMatchedContext context)
        {
            if (context.AccountBalanceNotificationExtendedQuery == null)
                return true;

            var extendedQuery = context.AccountBalanceNotificationExtendedQuery as RetailAccountBalanceNotificationExtendedQuery;
            if (extendedQuery == null)
                return false;

            if (extendedQuery.AccountIds != null && !extendedQuery.AccountIds.Contains(context.EventPayload.EntityId))
                return false;

            return true;
        }
    }
}
