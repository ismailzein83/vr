using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;

namespace TOne.WhS.AccountBalance.Business
{
    public class TOneAccountBalanceNotificationExtendedQuery : AccountBalanceNotificationExtendedQuery
    {
        public List<string> FinancialAccountIds { get; set; }
    }
}
