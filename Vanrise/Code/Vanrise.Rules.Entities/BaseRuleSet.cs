using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Entities
{
    public abstract class BaseRuleSet
    {
        public abstract BaseRule GetMatchedRule(Object target);

        public abstract bool AddRuleIfMatched(BaseRule rule);

        public abstract bool IsEmpty();

        public BaseRuleSet NextRuleSet { get; set; }
    }
}
