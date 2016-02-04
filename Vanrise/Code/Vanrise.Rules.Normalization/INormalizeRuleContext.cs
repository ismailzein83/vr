using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Normalization
{
    public interface INormalizeRuleContext
    {
        string Value { get; }

        string NormalizedValue { set; }
    }
}
