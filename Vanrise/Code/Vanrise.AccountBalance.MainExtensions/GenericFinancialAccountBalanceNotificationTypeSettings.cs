using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;

namespace Vanrise.AccountBalance.MainExtensions
{
    public class GenericFinancialAccountBalanceNotificationTypeSettings : AccountBalanceNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("B49238AC-54E2-4F3F-8C91-C864B7389566"); } }

        public override string NotificationQueryEditor { get { return "vr-accountbalance-financialaccountbalancenotificationtype-searcheditor"; } }

        public Guid FinancialAccountBEDefinitionId { get; set; }

        public override bool IsVRNotificationMatched(IAccountBalanceNotificationTypeIsMatchedContext context)
        {
            if (context.AccountBalanceNotificationExtendedQuery == null)
                return true;

            var extendedQuery = context.AccountBalanceNotificationExtendedQuery as GenericFinanciaAccountBalanceNotificationExtendedQuery;
            if (extendedQuery == null)
                return false;

            if (extendedQuery.AccountIds != null && !extendedQuery.AccountIds.Contains(context.EventPayload.EntityId))
                return false;

            return true;
        }
    }
    public class GenericFinanciaAccountBalanceNotificationExtendedQuery : AccountBalanceNotificationExtendedQuery
    {
        public List<string> AccountIds { get; set; }
    }
}
