using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountBalanceFinancialAccountPartNotificationTypeSettings : AccountBalanceNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("34BD051D-9BA3-4CBE-8E40-7D9C1E887F30"); } }
        public override string NotificationQueryEditor { get { return "retail-be-accountbalancefinancialaccountpartnotificationtype-searcheditor"; } }
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
