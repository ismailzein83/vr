using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountCreditLimit
    {
        public decimal? CreditLimit { get; set; }
        public int? CreditLimitCurrencyId { get; set; }
        public int? AccountCurrencyId { get; set; }
    }
}
