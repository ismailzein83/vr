using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class AllTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId => new Guid("E3A08709-67B9-4FCB-A54C-5AD11035A9FF");

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.FromTime = new DateTime(2000, 1, 1);
            context.ToTime = DateTime.Now;
        }
    }
}
