using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class LastDaysPeriod : VRTimePeriod
    {
        public int NumberOfDaysBack { get; set; }

        public override Guid ConfigId
        {
            get { return new Guid("1E862C38-72B1-4C3B-AC3F-F16510074684"); }
        }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.FromTime = context.EffectiveDate.AddDays(-1 * NumberOfDaysBack).Date;
            context.ToTime = context.EffectiveDate.Date;
        }
    }
}