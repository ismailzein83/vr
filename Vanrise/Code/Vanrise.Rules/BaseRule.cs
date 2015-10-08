using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules
{
    public abstract class BaseRule
    {
        public virtual bool IsAnyCriteriaExcluded(object target)
        {
            return false;
        }
    }
}
