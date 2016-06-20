using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartFinancial : AccountPartSettings
    {
        public PaymentMethod PaymentMethod { get; set; }

        public int CurrencyId { get; set; }

        public string BankDetails { get; set; }

        public AccountPostpaidSettings PostpaidSettings { get; set; }

        public AccountPrepaidSettings PrepardSettings { get; set; }
    }

    public class AccountPostpaidSettings
    {
        public int DuePeriodInDays { get; set; }
    }

    public class AccountPrepaidSettings
    {

    }
}
