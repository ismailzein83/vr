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
    public class LastYeasrPeriodTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("B0187271-297F-4544-B323-4469A13962AB"); } }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            PeriodDefinitionManager periodDefinitionManager = new PeriodDefinitionManager();
            var period = periodDefinitionManager.GetLastPeriod(context.EffectiveDate);
            if(period != null)
            {
                var effectivePeriod = periodDefinitionManager.GetLastPeriod(period.ToDate.AddYears(-1));
                if(effectivePeriod != null)
                {
                    context.FromTime = effectivePeriod.FromDate;
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
}
