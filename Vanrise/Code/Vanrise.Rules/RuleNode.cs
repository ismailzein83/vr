using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public class RuleNode
    {
        public List<IVRRule> Rules { get; set; }

        internal RuleNode ParentNode { get; set; }

        internal List<RuleNode> ChildNodes { get; set; }

        internal RuleNode UnMatchedRulesNode { get; set; }

        internal BaseRuleStructureBehavior Behavior { get; set; }

        internal bool IsUnMatchedRulesNode { get; set; }

        //public Dictionary<int, List<BaseRule>> Priorities { get; set; }
        public Dictionary<IVRRule, int> Priorities { get; set; }
    }
}
