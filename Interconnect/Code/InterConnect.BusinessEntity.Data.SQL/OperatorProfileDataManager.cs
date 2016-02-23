using InterConnect.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace InterConnect.BusinessEntity.Data.SQL
{
    public class OperatorProfileDataManager:BaseSQLDataManager,IOperatorProfileDataManager
    {
       
        #region ctor/Local Variables
        public OperatorProfileDataManager()
            : base(GetConnectionStringName("Interconnect_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(OperatorProfile operatorProfile, out int insertedId)
        {
            object operatorProfileId;

            int recordsEffected = ExecuteNonQuerySP("InterConnect_BE.sp_OperatorProfile_Insert", out operatorProfileId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings), operatorProfile.ExtendedSettingsRecordTypeId, Vanrise.Common.Serializer.Serialize(operatorProfile.ExtendedSettings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)operatorProfileId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(OperatorProfile operatorProfile)
        {
            int recordsEffected = ExecuteNonQuerySP("InterConnect_BE.sp_OperatorProfile_Update", operatorProfile.OperatorProfileId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings), operatorProfile.ExtendedSettingsRecordTypeId, Vanrise.Common.Serializer.Serialize(operatorProfile.ExtendedSettings));
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

        #region  Mappers
        private OperatorProfile OperatorProfileMapper(IDataReader reader)
        {
            OperatorProfile operatorProfile = new OperatorProfile
            {
                OperatorProfileId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<OperatorProfileSettings>(reader["Settings"] as string),
                ExtendedSettingsRecordTypeId = GetReaderValue<int?>(reader, "ExtendedSettingsRecordTypeID"),
                ExtendedSettings = reader["ExtendedSettings"] as string != null ? Vanrise.Common.Serializer.Deserialize<OperatorProfileSettings>(reader["ExtendedSettings"] as string) : null,
            };
            return operatorProfile;
        }

        #endregion
    }
}
