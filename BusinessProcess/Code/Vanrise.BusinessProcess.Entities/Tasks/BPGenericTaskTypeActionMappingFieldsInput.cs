using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public class BPGenericTaskTypeActionMappingFieldsInput
    {
        public string DecisionFieldName { get; set; }
        public string NotesFieldName { get; set; }
        public object DecisionFieldValue { get; set; }
        public object NotesFieldValue { get; set; }
        public Guid DataRecordTypeId { get; set; }
    }
}
