using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class VRWorkflowVariableType
    {
        public abstract Guid ConfigId { get; }

        public abstract Type GetRuntimeType(IVRWorkflowVariableTypeGetRuntimeTypeContext context);

        public abstract string GetRuntimeTypeDescription();
    }

    public interface IVRWorkflowVariableTypeGetRuntimeTypeContext
    {

    }
}
