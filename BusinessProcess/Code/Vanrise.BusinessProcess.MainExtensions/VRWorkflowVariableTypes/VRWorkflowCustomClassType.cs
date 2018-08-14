using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Entities;

namespace Vanrise.BusinessProcess.MainExtensions.VRWorkflowVariableTypes
{
    public class VRWorkflowCustomClassType : VRWorkflowVariableType
    {
        public override Guid ConfigId { get { return new Guid("A6078B0f-EFa2-414F-8a25-549628DA1762"); } }
        public VRCustomClassType FieldType { get; set; }

        public override Type GetRuntimeType(IVRWorkflowVariableTypeGetRuntimeTypeContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetRuntimeTypeDescription()
        {
            throw new NotImplementedException();
        }
    }
}
