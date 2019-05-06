using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class ContractAvailableServiceOutput
    {
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string ServicePackage { get; set; }
        public string ServiceType { get; set; }
        public Decimal? SubscriptionFee { get; set; }
        public Decimal AccessFee { get; set; }
        public bool NeedsProvisioning { get; set; }
        public bool IsNetwork { get; set; }
    }
}
