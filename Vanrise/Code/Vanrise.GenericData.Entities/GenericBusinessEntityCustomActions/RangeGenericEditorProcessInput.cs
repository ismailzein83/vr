using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class RangeGenericEditorProcessInput
    {
        public string RangeFieldName { get; set; }
        public Guid BusinessEntityDefinitionId { get; set; }
        public RangeGenericEditorDefinitionSettings Settings { get; set; }
        public Dictionary<string, object> FieldValues { get; set; }
    }
    public class RangeGenericEditorProcessMessage
    {
        public int NumberOfSucceededOperations { get; set; }
        public int NumberOfFailedOperations { get; set; }
    }
}
