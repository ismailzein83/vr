using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class LastMonthTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("F48249D1-C20D-4B36-BBEA-8AAF8A87B18E"); } }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            var newDate = context.EffectiveDate.AddMonths(-1);
            context.FromTime = new DateTime(newDate.Year, newDate.Month, 1);
            context.ToTime = new DateTime(newDate.Year,newDate.Month,newDate.GetLastDayOfMonth().Day);
        }
    }
}
