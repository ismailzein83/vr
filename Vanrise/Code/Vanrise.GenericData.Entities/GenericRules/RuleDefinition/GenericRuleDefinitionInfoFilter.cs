using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleDefinitionInfoFilter
    {
        public int RuleTypeId { get; set; }

        public List<IGenericRuleDefinitionFilter> Filters { get; set; }
    }
}
