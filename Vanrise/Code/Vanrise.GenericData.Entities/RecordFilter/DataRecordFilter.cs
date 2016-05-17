using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public enum RecordQueryLogicalOperator { And = 0, Or = 1 }
    public abstract class RecordFilter
    {
        public string FieldName { get; set; }
    }
    public class RecordFilterGroup : RecordFilter
    {
        public RecordQueryLogicalOperator LogicalOperator { get; set; }

        public List<RecordFilter> Filters { get; set; }
    }
   
}
