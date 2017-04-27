using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountBEFinancialAccountsSettings : BaseAccountExtendedSettings
    {
        public List<FinancialAccount> FinancialAccounts { get; set; }

        public int LastTakenSequenceNumber { get; set; }
    }
}
