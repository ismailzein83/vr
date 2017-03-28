using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public interface ISecurityRequiredPermissionSetManager : ISecBusinessManager
    {
        int GetRequiredPermissionSetId(string module, string requiredPermissionString);
    }
}
