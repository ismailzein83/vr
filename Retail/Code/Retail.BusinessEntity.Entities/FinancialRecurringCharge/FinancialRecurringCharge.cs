using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialRecurringCharge
    {
        public long ID { get; set; }
        public long RecurringChargeTypeId { get; set; }
        public string FinancialAccountId { get; set; }
        public decimal Amount { get; set; }
        public int CurrencyId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public FinancialRecurringChargePeriod RecurringChargePeriod { get; set; }
        public int? DuePeriod { get; set; }
    }
}
