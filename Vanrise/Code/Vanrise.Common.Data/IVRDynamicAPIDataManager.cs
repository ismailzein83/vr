using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRDynamicAPIDataManager : IDataManager
    {
        List<VRDynamicAPI> GetVRDynamicAPIs();
        bool AreVRDynamicAPIsUpdated(ref object updateHandle);
        bool Insert(VRDynamicAPI vrDynamicAPI, out int insertedId);
        bool Update(VRDynamicAPI vrDynamicAPI);

    }
}
