using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class VolCommitmentDetail : DealDefinitionDetail
    {
        public string CarrierAccountName { get; set; }

        public string TypeDescription { get; set; }

        public bool IsEffective { get; set; }
    }
}
