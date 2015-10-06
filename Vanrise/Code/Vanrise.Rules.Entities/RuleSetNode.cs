using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Entities
{
    public class RuleSetNode
    {
        public List<BaseRule> Rules { get; set; }

        public List<RuleSetNode> Children { get; set; }
    }
}
