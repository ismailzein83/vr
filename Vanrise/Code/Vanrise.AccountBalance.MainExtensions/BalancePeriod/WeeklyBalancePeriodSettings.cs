using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.MainExtensions.BalancePeriod
{
    public class WeeklyBalancePeriodSettings : BalancePeriodSettings
    {
        public DayOfWeek DayOfWeek { get; set; }

        public override void Execute(IBalancePeriodContext context)
        {
            var currentDate = DateTime.Today;
            if (currentDate.DayOfWeek != this.DayOfWeek)
            {
                int daysToAdd = this.DayOfWeek > currentDate.DayOfWeek ? (this.DayOfWeek - currentDate.DayOfWeek) : 7 + (this.DayOfWeek - currentDate.DayOfWeek);
                currentDate = currentDate.AddDays(daysToAdd);
            }
            context.NextPeriodDate = currentDate;
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
