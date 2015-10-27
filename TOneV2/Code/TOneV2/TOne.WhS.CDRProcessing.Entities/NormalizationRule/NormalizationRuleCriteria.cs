using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public enum NormalizationPhoneNumberType { CGPN = 1, CDPN = 2 }
    public class NormalizationRuleCriteria
    {
        public IEnumerable<NormalizationPhoneNumberType> PhoneNumberTypes { get; set; }

        public int? PhoneNumberLength { get; set; }

        public string PhoneNumberPrefix { get; set; }
    }
}
