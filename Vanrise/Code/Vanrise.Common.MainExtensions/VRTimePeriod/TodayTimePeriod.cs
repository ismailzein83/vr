using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class TodayTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("DE4F7720-5519-466C-8F14-E5F66A56DC42"); } }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.FromTime = context.EffectiveDate.Date;
            context.ToTime = context.EffectiveDate;
        }
    }
}