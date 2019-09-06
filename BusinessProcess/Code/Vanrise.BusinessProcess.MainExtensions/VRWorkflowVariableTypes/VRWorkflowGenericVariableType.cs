using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowVariableTypes
{
    public class VRWorkflowGenericVariableType : VRWorkflowVariableType
    {
        public override Guid ConfigId { get { return new Guid("A30953F6-D62E-4755-BDDE-DF87C0716864"); } }

        public Vanrise.GenericData.Entities.DataRecordFieldType FieldType { get; set; }

        public override string GetRuntimeTypeAsString(IVRWorkflowVariableTypeGetRuntimeTypeAsStringContext context)
        {
            return this.FieldType.GetRuntimeTypeAsString();
        }

        public override string GetRuntimeTypeDescription()
        {
            return FieldType.GetRuntimeTypeDescription();
        }

        public override bool IsValueType()
        {
            return this.FieldType.IsValueType();
        }
        public override GenericData.Entities.DataRecordFieldType GetFieldType()
        {
            return FieldType;
        }
    }
}
