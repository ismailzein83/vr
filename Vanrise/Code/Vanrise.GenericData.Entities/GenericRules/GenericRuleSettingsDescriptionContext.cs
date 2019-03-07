using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleSettingsDescriptionContext : IGenericRuleSettingsDescriptionContext
    {
        public GenericRuleDefinitionSettings RuleDefinitionSettings { get; set; }
    }

    public class GenericRuleIsRuleStillValidContext : IGenericRuleIsRuleStillValidContext
    {
        public GenericRuleDefinitionSettings RuleDefinitionSettings { get; set; }
    }
}
