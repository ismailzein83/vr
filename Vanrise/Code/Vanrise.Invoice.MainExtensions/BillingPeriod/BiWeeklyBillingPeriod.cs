using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public enum DailyType { 
        Monday = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6,
        
    }
    public class BiWeeklyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("66B9FD8C-DACB-4F35-9853-F6C0C9DFE4F1"); } }
        public DailyType DailyType { get; set; }
        public override DateTime GetPeriod(DateTime fromDate)
        {
            return fromDate.AddDays(14);
        }
    }
}
