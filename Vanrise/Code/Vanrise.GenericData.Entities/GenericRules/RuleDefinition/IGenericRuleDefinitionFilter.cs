using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public interface IGenericRuleDefinitionFilter
    {
        bool IsMatched(IGenericRuleDefinitionFilterContext context);
    }

    public interface IGenericRuleDefinitionFilterContext
    {
        GenericRuleDefinition RuleDefinition { get; }
    }
}
