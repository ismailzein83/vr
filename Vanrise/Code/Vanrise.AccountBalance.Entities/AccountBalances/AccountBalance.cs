using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalance
    {
        public long AccountBalanceId { get; set; }

        public Guid AccountTypeId { get; set; }

        public String AccountId { get; set; }

        public int CurrencyId { get; set; }

        public decimal InitialBalance { get; set; }

        public decimal CurrentBalance { get; set; }
    }

}
