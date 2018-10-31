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
            throw new NotImplementedException();
        }
    }
}
