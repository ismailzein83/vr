using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRLocalizationTextResourceDataManager : IDataManager
    {
        bool Update(VRLocalizationTextResource localizationTextResource);

        bool Insert(VRLocalizationTextResource localizationTextResource);

        List<VRLocalizationTextResource> GetVRLocalizationTextResources();

        bool AreVRLocalizationTextResourcesUpdated(ref object updateHandle);
    }
}
