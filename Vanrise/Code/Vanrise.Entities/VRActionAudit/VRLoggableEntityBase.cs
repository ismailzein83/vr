using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRLoggableEntityBase
    {
        public abstract string EntityUniqueName { get; }

        public abstract string ModuleName { get; }

        public abstract string EntityDisplayName { get; }

        public abstract string ViewHistoryItemClientActionName { get; }

        public abstract Object GetObjectId(IVRLoggableEntityGetObjectIdContext context);

        public abstract string GetObjectName(IVRLoggableEntityGetObjectNameContext context);

        public virtual VRActionAuditChangeInfoDefinition GetChangeInfoDefinition(IVRLoggableEntityGetChangeInfoDefinitionContext context)
        {
            return null;
        }
    }

    public interface IVRLoggableEntityGetObjectIdContext
    {
        Object Object { get; }
    }

    public interface IVRLoggableEntityGetObjectNameContext
    {
        Object Object { get; }
    }

    public interface IVRLoggableEntityGetChangeInfoDefinitionContext
    {
    }
}
