using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class PeriodBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("3D0DD9A7-3422-4311-B16F-08B03F175FAE"); } }
        public int NumberOfDays { get; set; }

        public override DateTime GetPeriod(DateTime fromDate)
        {
            return fromDate.AddDays(this.NumberOfDays);
        }
    }
}
