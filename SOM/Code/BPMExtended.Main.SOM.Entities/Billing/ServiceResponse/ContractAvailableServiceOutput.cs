using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class ContractAvailableServiceOutput
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PackageId { get; set; }
        public Decimal? SubscriptionFee { get; set; }
        public Decimal AccessFee { get; set; }
        public bool NeedsProvisioning { get; set; }
        public bool IsNetwork { get; set; }
    }
}
