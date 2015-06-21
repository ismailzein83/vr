using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteRuleTimeExecutionSetting
    {
        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public List<int> Days { get; set; }

        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
    }
}
