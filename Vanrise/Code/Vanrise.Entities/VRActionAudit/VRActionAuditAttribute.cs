using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRActionAuditAttribute : Attribute
    {
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

        string ActionDescription { set; }
    }
}
