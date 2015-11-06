using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class CustomerIdentificationRuleQuery
    {
        public string Description  { get; set; }
        public List<int> CustomerIds { get; set; }
        public string InTrunk  { get; set; }
         public string InCarrier  { get; set; }
         public string CDPN  { get; set; }
         public DateTime? EffectiveDate { get; set; }
    }
}
