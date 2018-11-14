using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRDynamicAPIModuleDataManager : IDataManager
    {
        bool AreVRDynamicAPIModulesUpdated(ref object updateHandle);

        List<VRDynamicAPIModule> GetVRDynamicAPIModules();

        bool Insert(VRDynamicAPIModule vrDynamicAPIModule, out int insertedId);

        bool Update(VRDynamicAPIModule vrDynamicAPIModule);


    }
}
