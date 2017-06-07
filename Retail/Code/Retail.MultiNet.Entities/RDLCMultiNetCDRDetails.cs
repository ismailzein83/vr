using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public class RDLCMultiNetCDRDetails
    {
        public string SubItemIdentifier { get; set; }
        public DateTime AttemptDateTime { get; set; }
        public decimal DurationInSeconds { get; set; }
        public string CallingNumber { get; set; }
        public string CalledNumber { get; set; }
        public decimal SaleAmount { get; set; }
        public RDLCMultiNetCDRDetails()
        {

        }
        public IEnumerable<RDLCMultiNetCDRDetails> GetRDLCMultiNetCDRDetailsSchema()
        {
            return null;
        }

    }
}
