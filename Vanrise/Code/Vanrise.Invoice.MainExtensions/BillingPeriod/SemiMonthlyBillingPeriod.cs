using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class SemiMonthlyBillingPeriod : BillingPeriod
    {
        public override Guid ConfigId { get { return new Guid("E0EDEB7A-FE1D-4207-A1E6-0AE2A42ED452"); } }

        public override BillingInterval GetPeriod(IBillingPeriodContext context)
        {
            throw new NotImplementedException();
        }
    }
}
