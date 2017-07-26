using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public enum TimeUnit { Hour = 0, Minute = 1 }
    public class LastTimePeriod : VRTimePeriod
    {
        public TimeUnit TimeUnit { get; set; }

        public int TimeValue { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("6C4A3B8D-0E1E-4141-9A21-7F7A68DC25BE"); }
        }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            switch (this.TimeUnit)
            {
                case TimeUnit.Hour: context.FromTime = context.EffectiveDate.Subtract(new TimeSpan(TimeValue, 0, 0)); break;
                case TimeUnit.Minute: context.FromTime = context.EffectiveDate.Subtract(new TimeSpan(0, TimeValue, 0)); break;
            }
            context.ToTime = context.EffectiveDate;
        }
    }
}