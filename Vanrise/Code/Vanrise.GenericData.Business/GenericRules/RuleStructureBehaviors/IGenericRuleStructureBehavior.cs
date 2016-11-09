using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Business.GenericRules.RuleStructureBehaviors
{
    public interface IGenericRuleStructureBehavior
    {
        GenericRuleDefinitionCriteriaField GenericRuleDefinitionCriteriaField { set; }
    }
}
