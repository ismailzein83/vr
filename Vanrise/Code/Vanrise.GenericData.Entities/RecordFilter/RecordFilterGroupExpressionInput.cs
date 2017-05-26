using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class RecordFilterGroupExpressionInput
    {
        public RecordFilterGroup FilterGroup { get; set; }
        public Dictionary<string, RecordFilterFieldInfo> RecordFilterFieldInfosByFieldName { get; set; } 
    }
}
