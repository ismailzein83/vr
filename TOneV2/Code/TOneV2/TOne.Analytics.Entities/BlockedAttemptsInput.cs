using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class BlockedAttemptsInput
    {
        public BlockedAttemptsFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public char GroupByNumber { get; set; }
    }
}
