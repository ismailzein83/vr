using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleDefinitionCriteriaGroup
    {
        public Guid GroupId { get; set; }
        public string GroupTitle { get; set; }
        public List<GenericRuleDefinitionCriteriaGroupField> Fields { get; set; }
    }

    public class GenericRuleDefinitionCriteriaGroupField
    {
        public string FieldName { get; set; }
    }
}
