using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TOne.WhS.Deal.Entities
{
    public enum VolCommitmentDealType
    {
        [Description("Buy")]
        Buy = 0,

        [Description("Sell")]
        Sell = 1
    }
    public class VolCommitmentDealQuery
    {
        public string Name { get; set; }
        public List<int> CarrierAccountIds { get; set; }
        public VolCommitmentDealType? Type { get; set; }
        public List<DealStatus> Status { get; set; }

    }
}
