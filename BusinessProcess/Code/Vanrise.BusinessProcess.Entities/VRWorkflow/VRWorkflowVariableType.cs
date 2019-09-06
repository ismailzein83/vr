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

        public abstract string GetRuntimeTypeAsString(IVRWorkflowVariableTypeGetRuntimeTypeAsStringContext context);

        public abstract bool IsValueType();

        public abstract string GetRuntimeTypeDescription();
        public virtual GenericData.Entities.DataRecordFieldType GetFieldType()
        {
            return null;
        }
    }

    public interface IVRWorkflowVariableTypeGetRuntimeTypeContext
    {

    }

    public interface IVRWorkflowVariableTypeGetRuntimeTypeAsStringContext
    {

    }
}
