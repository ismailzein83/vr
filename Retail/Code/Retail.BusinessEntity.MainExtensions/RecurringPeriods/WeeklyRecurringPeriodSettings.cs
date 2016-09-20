using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.RecurringPeriods
{
    public class WeeklyRecurringPeriodSettings : RecurringPeriodSettings
    {
        public override Guid ConfigId { get { return new Guid("d6d3fbcc-fb40-44e4-9fa1-dd7f1dbb6751"); } }

        public DayOfWeek DayOfWeek { get; set; }

        public override void Execute(IRecurringPeriodContext context)
        {
            var chargeDate = DateTime.Today;
            if (chargeDate.DayOfWeek != this.DayOfWeek)
            {
                int daysToAdd = this.DayOfWeek > chargeDate.DayOfWeek ? (this.DayOfWeek - chargeDate.DayOfWeek) : 7 + (this.DayOfWeek - chargeDate.DayOfWeek);
                chargeDate = chargeDate.AddDays(daysToAdd);
            }
            context.NextPeriodDate = chargeDate;
        }
    }
}
