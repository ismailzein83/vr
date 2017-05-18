using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class VRRule<T> where T : class
    {
        public long VRRuleId { get; set; }

        public Guid VRRuleDefinitionId { get; set; }

        public T Settings { get; set; }
    }
}
