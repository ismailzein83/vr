using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.MainExtensions
{
    public class SpecificDaysTimePeriod : VRTimePeriod
    {
        public override Guid ConfigId { get { return new Guid("FB9B7430-6FE8-418C-98EB-49730B562DE8"); } }
        public int DaysBack { get; set; }
        public int NumberOfDays { get; set; }
        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            DateTime fromTime = context.EffectiveDate.AddDays(-1 * DaysBack);
            context.FromTime = fromTime;
            DateTime toTime = new DateTime(fromTime.Year, fromTime.Month, fromTime.Day, 23, 59, 59, 998);
            context.ToTime = toTime.AddDays(NumberOfDays - 1);

        }
    }
}
