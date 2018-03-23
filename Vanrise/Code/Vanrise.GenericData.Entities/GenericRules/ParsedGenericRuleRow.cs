using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class ParsedGenericRuleRow
    {
        public int RowIndex { get; set; }
        public Dictionary<string, Object> ColumnValueByFieldName { get; set; }
    }
}
