using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public abstract class BaseRuleStructureBehavior
    {
        public abstract IEnumerable<RuleNode> StructureRules(IEnumerable<IVRRule> rules, out List<IVRRule> notMatchRules);

        public abstract List<RuleNode> GetMatchedNodes(object target);

        public abstract BaseRuleStructureBehavior CreateNewBehaviorObject();
    }
}
