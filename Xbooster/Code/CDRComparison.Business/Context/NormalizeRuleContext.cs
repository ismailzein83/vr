using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules.Normalization;

namespace CDRComparison.Business
{
    public class NormalizeRuleContext : INormalizeRuleContext
    {
        public string Value { get;  set; }
        public string NormalizedValue { get; set; }

        public Vanrise.Rules.IVRRule Rule
        {
            get;
            set;
        }
    }
}
