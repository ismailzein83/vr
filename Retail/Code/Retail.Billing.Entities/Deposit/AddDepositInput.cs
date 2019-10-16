using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Billing.Entities
{
    public class AddAdvancedPaymentInput
    {
        public long BillingAccountId { get; set; }

        public long? ContractId { get; set; }

        public long? ContractServiceId { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
    }

    public class AddAdvancedPaymentOutput
    {
        public long AdvancedPaymentId { get; set; }
    }

    public class AddDepositInput
    {
        public long BillingAccountId { get; set; }

        public long? ContractId { get; set; }

        public long? ContractServiceId { get; set; }

        public decimal Amount { get; set; }

        public int CurrencyId { get; set; }
    }

    public class AddInstallmentInput
    {
        public long BillingAccountId { get; set; }

        public Guid InstallmentTypeId { get; set; }

        public string ParentId { get; set; }

        public int CurrencyId { get; set; }

        public int NbOfItems { get; set; }
    }

    public class AddInstallmentOutput
    {
        public long InstallmentId { get; set; }
    }

    public class AddInstallmentItemInput
    {        
        public long InstallmentId { get; set; }

        public int ItemNumber { get; set; }

        public decimal Amount { get; set; }

        public DateTime ScheduleDate { get; set; }
    }

    public class AddInstallmentItemOutput
    {
        public long InstallmentItemId { get; set; }
    }
}
