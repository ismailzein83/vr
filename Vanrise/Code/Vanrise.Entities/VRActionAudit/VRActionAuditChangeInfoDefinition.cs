using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public abstract class VRActionAuditChangeInfoDefinition
    {
        public abstract string RuntimeEditor { get; set; }

        public abstract VRActionAuditChangeInfo ResolveChangeInfo(IVRActionAuditChangeInfoResolveChangeInfoContext context);
    }

    public interface IVRActionAuditChangeInfoResolveChangeInfoContext
    {
        Object OldObjectValue { get; }

        Object NewObjectValue { get; }

        string ChangeSummary { set; }
    }
}
