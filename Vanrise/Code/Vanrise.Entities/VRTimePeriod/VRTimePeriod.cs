using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRTimePeriod
    {
        public abstract Guid ConfigId { get;}
        public abstract void GetTimePeriod(IVRTimePeriodContext context);
    }
    public interface IVRTimePeriodContext
    {
        DateTime FromTime { set; }
        DateTime ToTime { set; }
    }
    public class VRTimePeriodContext : IVRTimePeriodContext
    {
        public DateTime FromTime { get;  set; }
        public DateTime ToTime { get;  set; }
    }
}
