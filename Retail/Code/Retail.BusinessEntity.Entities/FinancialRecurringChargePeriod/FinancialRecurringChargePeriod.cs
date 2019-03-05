using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class FinancialRecurringChargePeriod
    {
        public RecurringChargePeriodSettings Settings { get; set; }
    }
    public abstract class RecurringChargePeriodSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(IFinancialRecurringChargePeriodSettingsContext context);
    }
    public interface IFinancialRecurringChargePeriodSettingsContext
    {
        DateTime FromDate { get; }
        DateTime ToDate { get; }
        List<RecurringChargePeriodOutput> Periods { set; }
    }
}
