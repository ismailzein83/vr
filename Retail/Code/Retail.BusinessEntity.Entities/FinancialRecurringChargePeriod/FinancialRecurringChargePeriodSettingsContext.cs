using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialRecurringChargePeriodSettingsContext : IFinancialRecurringChargePeriodSettingsContext
    {
        public DateTime FromDate { set; get; }
        public DateTime ToDate { set; get; }
        public List<RecurringChargePeriodOutput> Periods { set; get; }
    }
    public class RecurringChargePeriodOutput
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public DateTime RecurringChargeDate { get; set; }
    }
}
