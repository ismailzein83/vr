using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public class RuleNode
    {
        public List<BaseRule> Rules { get; set; }

        internal List<RuleNode> ChildNodes { get; set; }

        internal RuleNode UnMatchedRulesNode { get; set; }

        internal BaseRuleStructureBehavior Behavior { get; set; }
    }
}
