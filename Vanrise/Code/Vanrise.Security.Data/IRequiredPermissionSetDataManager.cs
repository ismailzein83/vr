using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Data
{
    public interface IRequiredPermissionSetDataManager : IDataManager
    {
        int AddIfNotExists(string module, string requiredPermissionString);

        List<Entities.RequiredPermissionSet> GetAll();

        bool AreRequiredPermissionSetsUpdated(ref object updateHandle);
    }
}
