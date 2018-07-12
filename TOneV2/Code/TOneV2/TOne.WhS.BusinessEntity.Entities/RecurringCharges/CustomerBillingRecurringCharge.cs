using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerBillingRecurringCharge
    {
        public long ID { get; set; }
        public int FinancialAccountId { get; set; }
        public long RecurringChargeId { get; set; }
        public long InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CurrencyId { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public decimal VAT { get; set; }

    }
}
