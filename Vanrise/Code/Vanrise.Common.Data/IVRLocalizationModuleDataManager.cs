using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRLocalizationModuleDataManager : IDataManager
    {
        bool Update(VRLocalizationModule localizationModule);

        bool Insert(VRLocalizationModule localizationModule);

        List<VRLocalizationModule> GetVRLocalizationModules();

        bool AreVRLocalizationModulesUpdated(ref object updateHandle);
    }
}
