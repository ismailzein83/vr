using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class TopCarriersView
    {
        public string CarrierID { get; set; }
        public string CarrierName { get; set; }
        public decimal DurationInSeconds { get; set; }
        public int NumberOfCalls { get; set; }
    }
}
