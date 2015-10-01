using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities.Normalization.Actions
{
    public class AddPrefixActionSettings : NormalizationRuleActionSettings
    {
        public string Prefix { get; set; }
    }
}
