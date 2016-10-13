using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealSettings : DealSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("63C1310D-FDEA-4AC7-BDE1-58FD11E4EC65"); }
        }

        public int CarrierAccountId { get; set; }

        public List<SwapDealInbound> Inbounds { get; set; }

        public List<SwapDealOutbound> Outbounds { get; set; }
    }
}
