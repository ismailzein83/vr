using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartFinancial : AccountPartSettings, IAccountPayment
    {
        public const int ExtensionConfigId = 14;
        public PaymentMethod PaymentMethod { get; set; }

        public int CurrencyId { get; set; }

        public string BankDetails { get; set; }

        public AccountPostpaidSettings PostpaidSettings { get; set; }

        public AccountPrepaidSettings PrepaidSettings { get; set; }

        public int PaymentMethodId { get; set; }

        public int CreditClassId { get; set; }

        public int StatusChargingSetId { get; set; }

        public int BillingCycleId { get; set; }
    }

    public enum PaymentMethod { Prepaid = 0, Postpaid = 1 }

    public class AccountPostpaidSettings
    {
        public int DuePeriodInDays { get; set; }
    }

    public class AccountPrepaidSettings
    {

    }
}
