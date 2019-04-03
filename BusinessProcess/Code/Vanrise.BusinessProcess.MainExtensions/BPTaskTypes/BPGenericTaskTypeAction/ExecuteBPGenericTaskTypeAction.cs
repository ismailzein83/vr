using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.BusinessProcess.MainExtensions.BPTaskTypes;

namespace Vanrise.BusinessProcess.MainExtensions
{
    public class ExecuteBPGenericTaskTypeAction : BPGenericTaskTypeActionSettings
    {
        public override string ActionTypeName { get { return "Execute"; } }
        public override Guid ConfigId { get { return new Guid("36F6C817-A40F-4F8C-A2D6-9377EAF2169D"); } }
        public List<ExecuteBPGenericTaskTypeActionDefaultFieldValue> DefaultFieldValues { get; set; }
        public string DecisionMappingField { get; set; }
        public string NotesMappingField { get; set; }

    }
    public class ExecuteBPGenericTaskTypeActionDefaultFieldValue
    {
        public string FieldName { get; set; }
        public object FieldValue { get; set; }
    }
}
