using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class GetUpdateBulkActionsRuntimeInput
    {
        public Guid DataRecordTypeId { get; set; }
        public List<UpdateGenericBEField> Fields { get; set; } 

    }
    public class UpdateGenericBEField
    {
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public object DefaultValue { get; set; }
        public UpdateGenericBEFieldState FieldState { get; set; }
    }
    public enum UpdateGenericBEFieldState { ReadOnly = 1, Editable = 2, NotVisible = 3 }

}
