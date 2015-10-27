using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.CDRProcessing.Entities
{
    public class NormalizationRuleQuery
    {
        public List<NormalizationPhoneNumberType> PhoneNumberTypes { get; set; }
        public string PhoneNumberPrefix { get; set; }

        public int? PhoneNumberLength { get; set; }
        public string Description { get; set; }
    }
}
