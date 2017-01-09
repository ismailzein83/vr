using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public enum MonthlyType { SpecificDay = 0, LastDay = 1 }
    public class MonthlyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("37F03848-3F78-4A00-BA56-E6C7E2F5F3A2"); } }
        public List<MonthlyPeriod> MonthlyPeriods { get; set; }
        public override DateTime GetPeriod(DateTime fromDate)
        {
            return fromDate.AddMonths(1);
        }
    }
    public class MonthlyPeriod
    {
        public MonthlyType MonthlyType { get; set; }
        public int? SpecificDay { get; set; }

    }
}
