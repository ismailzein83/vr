using Retail.RA.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Retail.RA.Business
{
    class LastPeriodTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("6CF73654-686F-47C4-A93C-3EDDBEDC9676"); } }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            PeriodDefinitionManager periodDefinitionManager = new PeriodDefinitionManager();
            var period = periodDefinitionManager.GetLastPeriod(context.EffectiveDate);
            if (period != null)
            {
                context.FromTime = period.FromDate;
                context.ToTime = period.ToDate;
            }
            else
            {
                context.FromTime = context.EffectiveDate;
                context.ToTime = context.EffectiveDate;
            }
        }
    }
}
