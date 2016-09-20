using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.MainExtensions.BalancePeriod
{
    public class MonthlyBalancePeriodSettings : BalancePeriodSettings
    {
        public int DayOfMonth { get; set; }

        public override void Execute(IBalancePeriodContext context)
        {
            var currentDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, this.DayOfMonth);
            if (context.LastPeriodDate.HasValue && context.LastPeriodDate.Value >= currentDate)
                currentDate = currentDate.AddMonths(1);
            context.NextPeriodDate = currentDate;
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
