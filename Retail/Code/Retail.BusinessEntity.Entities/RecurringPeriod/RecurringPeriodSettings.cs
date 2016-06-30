using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public abstract class RecurringPeriodSettings
    {
        public int ConfigId { get; set; }

        public abstract void Execute(IRecurringPeriodContext context);
    }

    public interface IRecurringPeriodContext
    {
        DateTime ActivationDate { get; }

        DateTime? LastPeriodDate { get; }

        DateTime? NextPeriodDate { set; }
    }

    public class MonthlyRecurringPeriodSettings : RecurringPeriodSettings
    {
        public int DayOfMonth { get; set; }

        public override void Execute(IRecurringPeriodContext context)
        {
            var chargeDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, this.DayOfMonth);
            if (context.LastPeriodDate.HasValue && context.LastPeriodDate.Value >= chargeDate)
                chargeDate = chargeDate.AddMonths(1);
            context.NextPeriodDate = chargeDate;
        }
    }

    public class WeeklyRecurringPeriodSettings : RecurringPeriodSettings
    {
        public DayOfWeek DayOfWeek { get; set; }

        public override void Execute(IRecurringPeriodContext context)
        {
            var chargeDate = DateTime.Today;
            if(chargeDate.DayOfWeek != this.DayOfWeek)
            {
                int daysToAdd = this.DayOfWeek > chargeDate.DayOfWeek ? (this.DayOfWeek - chargeDate.DayOfWeek) : 7 + (this.DayOfWeek - chargeDate.DayOfWeek);
                chargeDate = chargeDate.AddDays(daysToAdd);
            }
            context.NextPeriodDate = chargeDate;
        }
    }

}
