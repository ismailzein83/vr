using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BiMonthlyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("94AA2673-04E4-4EFC-9913-CD95C40CD600"); } }

        public override DateTime GetPeriod(DateTime fromDate)
        {
            return fromDate.AddMonths(2);
        }
    }
}
