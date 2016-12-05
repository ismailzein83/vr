using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BiWeeklyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("66B9FD8C-DACB-4F35-9853-F6C0C9DFE4F1"); } }

        public override DateTime GetPeriod(DateTime fromDate)
        {
            return fromDate.AddDays(14);
        }
    }
}
