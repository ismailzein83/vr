using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public abstract class BaseRule
    {
        public int RuleId { get; set; }

        public virtual bool IsAnyCriteriaExcluded(object target)
        {
            return false;
        }

        public DateTime BeginEffectiveTime { get; set; }

        public DateTime? EndEffectiveTime { get; set; }
    }

   

}
