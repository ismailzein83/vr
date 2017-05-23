using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRRule
    {
        public long VRRuleId { get; set; }

        public Guid VRRuleDefinitionId { get; set; }

        public VRRuleSettings Settings { get; set; }
    }

    public abstract class VRRuleSettings
    {

    }

    public class VRRule<T> where T : VRRuleSettings
    {
        public long VRRuleId { get; set; }

        public Guid VRRuleDefinitionId { get; set; }

        public T Settings { get; set; }
    }
}
