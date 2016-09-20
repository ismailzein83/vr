using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public class BaseRuleTarget
    {
        public virtual DateTime? EffectiveOn { get; set; }

        public virtual bool IsEffectiveInFuture { get; set; }
    }
}
