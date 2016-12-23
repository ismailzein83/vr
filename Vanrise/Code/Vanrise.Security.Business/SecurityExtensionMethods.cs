using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public static class SecurityExtensionMethods
    {
        public static bool ComparePermissionEntityId(this Permission pemission, string entityId)
        {
            return pemission.EntityId.Equals(entityId, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
