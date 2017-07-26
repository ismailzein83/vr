using System;

namespace Vanrise.Entities
{
    public abstract class VRTimePeriod
    {
        public abstract Guid ConfigId { get; }
        public abstract void GetTimePeriod(IVRTimePeriodContext context);
    }
    public interface IVRTimePeriodContext
    {
        DateTime EffectiveDate { get; }
        DateTime FromTime { set; }
        DateTime ToTime { set; }
    }
    public class VRTimePeriodContext : IVRTimePeriodContext
    {
        public DateTime EffectiveDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
    }
}