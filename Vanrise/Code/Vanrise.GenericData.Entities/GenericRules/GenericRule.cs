using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public abstract class GenericRule : Vanrise.Rules.BaseRule, IGenericRule
    {
        public Guid DefinitionId { get; set; }

        public GenericRuleCriteria Criteria { get; set; }

        public abstract string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context);

        public abstract bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue);
    }

    public interface IGenericRule
    {
        GenericRuleCriteria Criteria { get; }
    }
    public interface ICreateGenericRuleFromExcelContext
    {
        GenericRule GenericRule { set; }

        string ErrorMessage { set; }

        Dictionary<string, Object> ParsedGenericRulesFields { get; }
    }
}
