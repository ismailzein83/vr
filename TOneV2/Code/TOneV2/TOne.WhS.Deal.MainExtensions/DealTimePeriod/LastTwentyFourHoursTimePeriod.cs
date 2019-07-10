using System;
using Vanrise.Entities;

namespace TOne.WhS.Deal.MainExtensions.DealTimePeriod
{
    public class LastTwentyFourHoursTimePeriod : TOne.WhS.Deal.Entities.DealTimePeriod 
    {
        public override Guid ConfigId => new Guid("215017D3-256F-4AFD-AF99-4266DD4D0AC4");

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.ToTime = context.EffectiveDate;
            context.FromTime = context.EffectiveDate.AddDays(-1);
        }
    } 
}