using System;
using Vanrise.Entities;

namespace TOne.WhS.Deal.MainExtensions.DealTimePeriod
{
    public class WeeklyTimePeriod : TOne.WhS.Deal.Entities.DealTimePeriod
    {
        public override Guid ConfigId => new Guid("3C5CCF5B-224E-4502-AFA4-2F18A8E39660");

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.ToTime = context.EffectiveDate;
            context.FromTime = context.EffectiveDate.AddDays(-7);
        }
    }
}