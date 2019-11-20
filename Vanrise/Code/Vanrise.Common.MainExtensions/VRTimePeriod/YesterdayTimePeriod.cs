using System;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class YesterdayTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("D72E97C0-73BA-4BB9-AA37-4889A4C3386F"); } }

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            DateTime yesterday = context.EffectiveDate.Date.AddDays(-1);
            context.FromTime = yesterday;
            context.ToTime = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, 23, 59, 59, 998);
        }

        public override string GetDescription(IVRTimePeriodGetDescriptionContext context)
        {
            return "Yesterday";
        }
    }
}
