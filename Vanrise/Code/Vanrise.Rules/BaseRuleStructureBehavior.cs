using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public abstract class BaseRuleStructureBehavior
    {
        public abstract IEnumerable<RuleNode> StructureRules(IEnumerable<BaseRule> rules, out List<BaseRule> notMatchRules);

        public abstract List<RuleNode> GetMatchedNodes(object target);

        public abstract BaseRuleStructureBehavior CreateNewBehaviorObject();
    }
}
