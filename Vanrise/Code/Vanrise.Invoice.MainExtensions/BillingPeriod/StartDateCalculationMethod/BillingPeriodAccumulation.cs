using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Vanrise.Invoice.MainExtensions
{
    public class BillingPeriodAccumulation:StartDateCalculationMethod
    {
        public override Guid ConfigId { get { return new Guid("5F9DDA2C-860D-42E6-BDF2-904B1B8FF287"); } }

        public override void CalculateDate(IStartDateCalculationMethodContext context)
        {
            DateTime fromDate = context.InitialStartDate;
            DateTime toDate = context.BillingPeriod.GetPeriod(fromDate);
            while ((context.PartnerCreatedDate >= fromDate  && context.PartnerCreatedDate <= toDate))
            {
                fromDate = toDate.AddDays(1);
                toDate = context.BillingPeriod.GetPeriod(fromDate);
            }
            context.FromDate = fromDate;
            context.ToDate = toDate;
        }
    }
}
