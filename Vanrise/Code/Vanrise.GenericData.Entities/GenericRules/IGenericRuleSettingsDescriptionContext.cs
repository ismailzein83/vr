using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IGenericRuleSettingsDescriptionContext
    {
        GenericRuleDefinitionSettings RuleDefinitionSettings { get; set; }
    }

    public interface IGenericRuleIsRuleStillValidContext
    {
        GenericRuleDefinitionSettings RuleDefinitionSettings { get; }
    }
}
