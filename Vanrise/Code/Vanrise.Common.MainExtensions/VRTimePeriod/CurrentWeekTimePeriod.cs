using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class CurrentWeekTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId => new Guid("858E92C1-9FBA-45A6-B4BF-9458E53C2EC8");
        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            DateTime startOfWeek = GetStartOfWeek(context.EffectiveDate, DayOfWeek.Monday);
            context.FromTime = startOfWeek;
            context.ToTime = context.EffectiveDate;
        }
        public DateTime GetStartOfWeek(DateTime currentDate, DayOfWeek startOfWeek)
        {
            return currentDate.AddDays(-(DateTime.Today.DayOfWeek - startOfWeek)).Date;
        }
    }
}
