using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.GenericData.Entities;

namespace InterConnect.BusinessEntity.Data.SQL
{
    public class OperatorProfileDataManager:BaseSQLDataManager,IOperatorProfileDataManager
    {
        #region ctor/Local Variables

        IDataRecordTypeManager _recordTypeManager = BusinessManagerFactory.GetManager<IDataRecordTypeManager>();
        
        public OperatorProfileDataManager()
            : base(GetConnectionStringName("Interconnect_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        
        #endregion

        #region Public Methods
        public bool Insert(OperatorProfile operatorProfile, out int insertedId)
        {
            object operatorProfileId;

            int recordsEffected = ExecuteNonQuerySP("InterConnect_BE.sp_OperatorProfile_Insert", out operatorProfileId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings), operatorProfile.ExtendedSettingsRecordTypeId, GetSerializedExtendedSettings(operatorProfile));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)operatorProfileId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(OperatorProfile operatorProfile)
        {
            int recordsEffected = ExecuteNonQuerySP("InterConnect_BE.sp_OperatorProfile_Update", operatorProfile.OperatorProfileId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings), operatorProfile.ExtendedSettingsRecordTypeId, GetSerializedExtendedSettings(operatorProfile));
            return (recordsEffected > 0);
        }
        public bool AreOperatorProfilesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[InterConnect_BE].[OperatorProfile]", ref updateHandle);
        }
        public List<OperatorProfile> GetOperatorProfiles()
        {
            return GetItemsSP("InterConnect_BE.sp_OperatorProfile_GetAll", OperatorProfileMapper);
        }
        #endregion

        #region Private Methods

        string GetSerializedExtendedSettings(OperatorProfile operatorProfile)
        {
            if (operatorProfile.ExtendedSettingsRecordTypeId == null || operatorProfile.ExtendedSettings == null)
                return null;
            return _recordTypeManager.SerializeRecord(operatorProfile.ExtendedSettings, (int)operatorProfile.ExtendedSettingsRecordTypeId);
        }
        
        #endregion

        #region  Mappers
        private OperatorProfile OperatorProfileMapper(IDataReader reader)
        {
            OperatorProfile operatorProfile = new OperatorProfile
            {
                OperatorProfileId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<OperatorProfileSettings>(reader["Settings"] as string),
                ExtendedSettingsRecordTypeId = GetReaderValue<int?>(reader, "ExtendedSettingsRecordTypeID")
            };
            var extendedSettings = reader["ExtendedSettings"] as string;
            if (operatorProfile.ExtendedSettingsRecordTypeId != null && extendedSettings != null)
            {
                operatorProfile.ExtendedSettings = _recordTypeManager.DeserializeRecord(reader["ExtendedSettings"] as string, (int)operatorProfile.ExtendedSettingsRecordTypeId);
            }
            return operatorProfile;
        }

        #endregion
    }
}
