using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Cost.Entities
{
    public class ProcessedCDRCost
    {
        public DateTime AttemptDateTime { get; set; }

        public string CGPN { get; set; }

        public string CDPN { get; set; }

        public double DurationInSeconds { get; set; }

        public double Rate { get; set; }

        public double Amount { get; set; }
    }
}
