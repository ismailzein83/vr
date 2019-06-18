using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.FinancialRecurringChargePeriod
{
    public class BillingCycleRecurringCharge : RecurringChargePeriodSettings
    {
        public override Guid ConfigId { get { return new Guid("01FD5D1B-484B-4AD4-887C-CE3843A96060"); } }

        public override void Execute(IFinancialRecurringChargePeriodSettingsContext context)
        {
            context.Periods = new List<RecurringChargePeriodOutput>
            {
                new RecurringChargePeriodOutput
                {
                    From = context.FromDate,
                    To = context.ToDate,
                    RecurringChargeDate = context.ToDate
                }
            };
        }
    }
}
