using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public enum VolCommitmentDealType { Buy = 0, Sell = 1 }
    public class VolCommitmentDealSettings : DealSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("B606E88C-4AE5-4BF0-BCE5-10D456A092F5"); }
        }

        public VolCommitmentDealType DealType { get; set; }

        public int CarrierAccountId { get; set; }

        public List<VolCommitmentDealItem> Items { get; set; }
    }
}
