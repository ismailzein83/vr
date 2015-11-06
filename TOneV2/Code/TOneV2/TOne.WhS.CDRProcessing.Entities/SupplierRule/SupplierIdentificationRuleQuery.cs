using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class SupplierIdentificationRuleQuery
    {
        public string Description{ get; set; }
        public List<int> SupplierIds { get; set; }
        public string OutTrunk { get; set; }
        public string OutCarrier { get; set; }
        public string CDPN { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
