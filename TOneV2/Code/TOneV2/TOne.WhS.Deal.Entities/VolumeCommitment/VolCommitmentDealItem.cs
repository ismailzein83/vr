using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class VolCommitmentDealItem
    {
        public string Name { get; set; }

        public int CountryId { get; set; }

        public List<long> ZoneIds { get; set; }

        public List<VolCommitmentDealItemTier> Tiers { get; set; }
    }


}
