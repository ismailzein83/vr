using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface IVRLocalizationLanguageDataManager : IDataManager
    {
        bool Update(VRLocalizationLanguage localizationLanguage);

        bool Insert(VRLocalizationLanguage localizationLanguage);
        
        List<VRLocalizationLanguage> GetVRLocalizationLanguages();

        bool AreVRLocalizationLanguagesUpdated(ref object updateHandle);
    }
}
