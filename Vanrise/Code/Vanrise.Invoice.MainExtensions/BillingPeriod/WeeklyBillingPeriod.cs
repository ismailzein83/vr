using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class WeeklyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("A08230D0-317E-4B30-A5EA-5ED72E2604D8"); } }
        public DailyType DailyType { get; set; }
        public override DateTime GetPeriod(DateTime fromDate)
        {
            return fromDate.AddDays(7);
        }
    }
}
