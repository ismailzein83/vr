using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleRowToAdd
    {
        public int RowIndex { get; set; }

        public GenericRule RuleToAdd { get; set; }

        public List<GenericRule> RulesToClose{ get; set; }
    }
}
