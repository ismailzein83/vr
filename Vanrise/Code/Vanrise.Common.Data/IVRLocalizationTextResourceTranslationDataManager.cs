using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRLocalizationTextResourceTranslationDataManager : IDataManager
    {
        List<VRLocalizationTextResourceTranslation> GetVRLocalizationTextResourcesTranslation();
        bool AreVRLocalizationTextResourcesTranslationUpdated(ref object updateHandle);
        bool AddVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation localizationTextResource);
        bool UpdateVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation localizationTextResource);
    }
}