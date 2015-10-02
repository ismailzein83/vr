using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Entities
{
    public abstract class BaseRule
    {
        public abstract bool EvaluateAdvancedConditions(Object target);
    }
}
