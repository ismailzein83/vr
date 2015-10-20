using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public class BaseRuleTarget
    {
        public DateTime? EffectiveOn { get; set; }

        public bool IsEffectiveInFuture { get; set; }
    }
}
