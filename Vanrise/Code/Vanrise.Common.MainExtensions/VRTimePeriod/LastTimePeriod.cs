using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public enum TimeUnit { Day = 0, Hour = 1, Minute = 2 }
    public enum StartingFrom { ExecutionTime = 0, Midnight = 1 }
    public class LastTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("6C4A3B8D-0E1E-4141-9A21-7F7A68DC25BE"); } }

        public TimeUnit TimeUnit { get; set; }

        public StartingFrom StartingFrom { get; set; }

        public int TimeValue { get; set; }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            DateTime effectiveDate;
            switch (this.StartingFrom)
            {
                case StartingFrom.ExecutionTime: effectiveDate = context.EffectiveDate; break;
                case StartingFrom.Midnight: effectiveDate = context.EffectiveDate.Date; break;
                default: throw new NotSupportedException(string.Format("StartingFrom {0} not supported.", this.StartingFrom));
            }

            switch (this.TimeUnit)
            {
                case TimeUnit.Day: context.FromTime = effectiveDate.Subtract(new TimeSpan(TimeValue, 0, 0, 0)); break;
                case TimeUnit.Hour: context.FromTime = effectiveDate.Subtract(new TimeSpan(TimeValue, 0, 0)); break;
                case TimeUnit.Minute: context.FromTime = effectiveDate.Subtract(new TimeSpan(0, TimeValue, 0)); break;
            }
            context.ToTime = effectiveDate;
        }
    }
}