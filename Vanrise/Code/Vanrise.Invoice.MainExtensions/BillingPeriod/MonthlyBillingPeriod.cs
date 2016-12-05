using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class MonthlyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("37F03848-3F78-4A00-BA56-E6C7E2F5F3A2"); } }

        public override DateTime GetPeriod(DateTime fromDate)
        {
            return fromDate.AddMonths(1);
        }
    }
}
