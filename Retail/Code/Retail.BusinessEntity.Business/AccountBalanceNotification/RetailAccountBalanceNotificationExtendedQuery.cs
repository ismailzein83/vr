using System.Collections.Generic;
using Vanrise.AccountBalance.Business;

namespace Retail.BusinessEntity.Business
{
    public class RetailAccountBalanceNotificationExtendedQuery : AccountBalanceNotificationExtendedQuery
    {
        public List<string> AccountIds { get; set; }
    }
}
