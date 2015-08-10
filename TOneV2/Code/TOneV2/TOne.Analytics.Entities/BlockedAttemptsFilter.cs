using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
   public class BlockedAttemptsFilter
    {
        public List<int> SwitchIds { get; set; }
        public List<string> CustomerIds { get; set; }
        public List<int> ZoneIds { get; set; }
    }
}
