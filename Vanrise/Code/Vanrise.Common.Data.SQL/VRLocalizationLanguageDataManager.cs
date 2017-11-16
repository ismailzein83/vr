﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using System.Data;


namespace Vanrise.Common.Data.SQL
{
    public class VRLocalizationLanguageDataManager:BaseSQLDataManager,IVRLocalizationLanguageDataManager
    {
        public VRLocalizationLanguageDataManager()
            : base(GetConnectionStringName("VRLocalizationDBDBConnStringKey", "VRLocalizationDBDBConnString"))
        {

        }
        public List<VRLocalizationLanguage> GetVRLocalizationLanguages()
        {
            return GetItemsSP("[VRLocalization].[sp_Language_GetAll]", VRLocalizationLanguageMapper);
        }
         public bool Update(VRLocalizationLanguage localizationLanguage)
        {
             return ExecuteNonQuerySP("[VRLocalization].[sp_Language_Update]", localizationLanguage.VRLanguageId, localizationLanguage.Name) > 0;
        }

        public bool Insert(VRLocalizationLanguage localizationLanguage)
        {
            return ExecuteNonQuerySP("[VRLocalization].[sp_Language_Insert]", localizationLanguage.VRLanguageId, localizationLanguage.Name) > 0;
        }


        public bool AreVRLocalizationLanguagesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[VRLocalization].[Language]", ref updateHandle);
        }

        private VRLocalizationLanguage VRLocalizationLanguageMapper(IDataReader reader)
        {
            VRLocalizationLanguage vrLocalizationLanguage = new VRLocalizationLanguage
            {
                VRLanguageId = (Guid)reader["ID"],
                Name = reader["Name"] as string,
                ParentLanguageId = GetReaderValue<Guid?>(reader,"ParentLanguageID")        
            };

            return vrLocalizationLanguage;
        }
    }
}
