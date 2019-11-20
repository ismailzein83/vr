using System;
using System.ComponentModel;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public enum TimeUnit { Year = 0, Month = 1, Day = 2, Hour = 3, Minute = 4, }

    public enum StartingFrom
    {
        [Description("Execution Time")]
        ExecutionTime = 0,
        [Description("Midnight")]
        Midnight = 1,
        [Description("Execution Time With Offset")]
        ExecutionTimeWithOffset = 2
    }

    public class LastTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("6C4A3B8D-0E1E-4141-9A21-7F7A68DC25BE"); } }

        public TimeUnit TimeUnit { get; set; }

        public StartingFrom StartingFrom { get; set; }

        public int TimeValue { get; set; }

        public TimeUnit? OffsetTimeUnit { get; set; }

        public int? OffsetValue { get; set; }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            DateTime effectiveDate;

            switch (this.StartingFrom)
            {
                case StartingFrom.ExecutionTime: effectiveDate = context.EffectiveDate; break;
                case StartingFrom.Midnight: effectiveDate = context.EffectiveDate.Date; break;
                case StartingFrom.ExecutionTimeWithOffset:
                    effectiveDate = context.EffectiveDate;
                    switch (this.OffsetTimeUnit.Value)
                    {
                        case TimeUnit.Year: effectiveDate = effectiveDate.AddYears(-OffsetValue.Value); break;
                        case TimeUnit.Month: effectiveDate = effectiveDate.AddMonths(-OffsetValue.Value); break;
                        case TimeUnit.Day: effectiveDate = effectiveDate.AddDays(-OffsetValue.Value); break;
                        case TimeUnit.Hour: effectiveDate = effectiveDate.AddHours(-OffsetValue.Value); break;
                        case TimeUnit.Minute: effectiveDate = effectiveDate.AddMinutes(-OffsetValue.Value); break;
                        default: throw new NotSupportedException($"Offset time unit {OffsetTimeUnit.Value} not supported.");
                    }
                    break;
                default: throw new NotSupportedException(string.Format("StartingFrom {0} not supported.", this.StartingFrom));
            }

            switch (this.TimeUnit)
            {
                case TimeUnit.Year: context.FromTime = effectiveDate.AddYears(-TimeValue); break;
                case TimeUnit.Month: context.FromTime = effectiveDate.AddMonths(-TimeValue); break;
                case TimeUnit.Day: context.FromTime = effectiveDate.AddDays(-TimeValue); break;
                case TimeUnit.Hour: context.FromTime = effectiveDate.AddHours(-TimeValue); break;
                case TimeUnit.Minute: context.FromTime = effectiveDate.AddMinutes(-TimeValue); break;
                default: throw new NotSupportedException($"Time unit {TimeUnit} not supported.");
            }

            context.ToTime = effectiveDate;
        }

        public override string GetDescription(IVRTimePeriodGetDescriptionContext context)
        {
            string offsetString = OffsetValue.HasValue ? $" {OffsetValue} {OffsetTimeUnit}{(OffsetValue > 1 ? "s" : "")}" : "";
            return $"Last {TimeValue} {TimeUnit}{(TimeValue > 1 ? "s" : "")} starting from {Utilities.GetEnumDescription(StartingFrom)}{offsetString}";
        }
    }
}