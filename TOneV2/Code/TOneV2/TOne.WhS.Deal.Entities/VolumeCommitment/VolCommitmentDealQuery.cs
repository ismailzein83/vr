using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TOne.WhS.Deal.Entities
{
    public class VolCommitmentDealQuery
    {
        public string Name { get; set; }

        public List<int> CarrierAccountIds { get; set; }

        public List<VolCommitmentDealType> Types { get; set; }

    }
}
