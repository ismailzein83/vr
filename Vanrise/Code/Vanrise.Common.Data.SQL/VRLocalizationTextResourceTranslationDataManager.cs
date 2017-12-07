using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;

namespace Vanrise.Common.Data.SQL
{
    public class VRLocalizationTextResourceTranslationDataManager : BaseSQLDataManager, IVRLocalizationTextResourceTranslationDataManager
    {
        #region Public Methods
        public VRLocalizationTextResourceTranslationDataManager()
            : base(GetConnectionStringName("VRLocalizationDBConnStringKey", "VRLocalizationDBConnString"))
        {

        }
        public List<VRLocalizationTextResourceTranslation> GetVRLocalizationTextResourcesTranslation()
        {
            return GetItemsSP("[VRLocalization].[sp_TextResourceTranslation_GetAll]", VRLocalizationTextResourceTranslationMapper);
        }
        public bool AreVRLocalizationTextResourcesTranslationUpdated(ref object updateHandle)
        {
             return base.IsDataUpdated("[VRLocalization].[TextResourceTranslation]", ref updateHandle);
        }
        public bool UpdateVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation localizationTextResourceTranslation)
        {
            return ExecuteNonQuerySP("[VRLocalization].[sp_TextResourceTranslation_Update]", localizationTextResourceTranslation.VRLocalizationTextResourceTranslationId, localizationTextResourceTranslation.ResourceId, localizationTextResourceTranslation.LanguageId) > 0;
        }

        public bool AddVRLocalizationTextResourceTranslation(VRLocalizationTextResourceTranslation localizationTextResourceTranslation)
        {
            return ExecuteNonQuerySP("[VRLocalization].[sp_TextResourceTranslation_Insert]", localizationTextResourceTranslation.VRLocalizationTextResourceTranslationId, localizationTextResourceTranslation.ResourceId, localizationTextResourceTranslation.LanguageId) > 0;
        }
        #endregion

        #region Private Methods
        private VRLocalizationTextResourceTranslation VRLocalizationTextResourceTranslationMapper(IDataReader reader)
        {
            VRLocalizationTextResourceTranslation vrLocalizationTextResource = new VRLocalizationTextResourceTranslation
            {
                VRLocalizationTextResourceTranslationId = GetReaderValue<Guid>(reader, "ID"),
                ResourceId = GetReaderValue<Guid>(reader, "TextResourceID"),
                LanguageId = GetReaderValue<Guid>(reader, "LanguageID"),
                Settings = Vanrise.Common.Serializer.Deserialize<VRLocalizationTextResourceTranslationSettings>(reader["Settings"] as string),
            };

            return vrLocalizationTextResource;
        }
        #endregion

      
    }
}
