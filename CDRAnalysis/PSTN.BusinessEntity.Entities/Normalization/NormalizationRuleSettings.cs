using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public class NormalizationRuleSettings
    {
        public int ConfigId { get; set; }

        public List<NormalizationRuleActionSettings> Actions { get; set; }
    }
}
