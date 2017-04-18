using System;
using System.Collections.Generic;
using Vanrise.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.Business
{
    public class TOneAccountBalanceNotificationExtendedQuery : AccountBalanceNotificationExtendedQuery
    {
        public Guid? AccountTypeId { get; set; }

        public List<string> FinancialAccountIds { get; set; } 
    }
}
