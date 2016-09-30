using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class GenericRule : Vanrise.Rules.BaseRule
    {
        public Guid DefinitionId { get; set; }

        public GenericRuleCriteria Criteria { get; set; }

        public abstract string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context);

        public abstract bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue);
    }
}
