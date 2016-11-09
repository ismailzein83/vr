using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;
using Vanrise.Rules.Normalization;

namespace Vanrise.GenericData.Normalization
{
    public class NormalizeRuleContext : INormalizeRuleContext
    {
        public string Value
        {
            get;
            set;
        }

        public string NormalizedValue
        {
            get;
            set;
        }

       public IVRRule Rule { get; set; }
    }
}
