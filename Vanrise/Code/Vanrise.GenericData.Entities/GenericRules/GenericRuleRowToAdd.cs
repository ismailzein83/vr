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
        public Dictionary<string, Object> CriteriasByFieldName { get; set; }
        public string Description { get; set; }
    }
}
