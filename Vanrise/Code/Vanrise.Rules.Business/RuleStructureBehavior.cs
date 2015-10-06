using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules.Entities;

namespace Vanrise.Rules.Business
{
    public abstract class RuleStructureBehavior
    {
        public abstract IEnumerable<RuleNode> StructureRules(IEnumerable<BaseRule> rules, out List<BaseRule> notMatchRules);

        public abstract RuleNode GetMatchedNode(object target);
    }
}
