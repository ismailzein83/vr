using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.RecurringPeriods
{
    public class MonthlyRecurringPeriodSettings : RecurringPeriodSettings
    {
        public override Guid ConfigId { get { return new Guid("91e3809b-244f-4ee8-8487-567e10e2fbc7"); } }
        public int DayOfMonth { get; set; }

        public override void Execute(IRecurringPeriodContext context)
        {
            var chargeDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, this.DayOfMonth);
            if (context.LastPeriodDate.HasValue && context.LastPeriodDate.Value >= chargeDate)
                chargeDate = chargeDate.AddMonths(1);
            context.NextPeriodDate = chargeDate;
        }
    }
}
