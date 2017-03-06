using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRActionAuditAttribute : Attribute
    {
        public Type ObjectNameResolverType { get; set; }
        public abstract void GetAuditDetails(IVRActionAuditAttributeContext context);
    }

    public interface IVRActionAuditAttributeContext
    {
        string ActionURL { get; }

        T GetActionArgument<T>(string argumentName);

        string ModuleName { set; }

        string EntityName { set; }

        string ActionName { set; }

        string ObjectId { set; }

        string ObjectName { set; }

        string ActionDescription { set; }
    }

    public interface IVRActionObjectNameResolver
    {
        string GetObjectName(IVRActionObjectNameResolverContext context);
    } 

    public interface IVRActionObjectNameResolverContext
    {
        string ObjectId { get; }
    }
}
