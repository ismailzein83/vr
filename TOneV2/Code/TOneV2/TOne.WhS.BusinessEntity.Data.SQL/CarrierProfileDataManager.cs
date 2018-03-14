using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Data.SQL;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public class CarrierProfileDataManager : BaseSQLDataManager, ICarrierProfileDataManager
    {
   
        #region ctor/Local Variables
        public CarrierProfileDataManager()
            : base(GetConnectionStringName("TOneWhS_BE_DBConnStringKey", "TOneWhS_BE_DBConnString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(CarrierProfile carrierProfile, out int insertedId)
        {
            object carrierProfileId;

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierProfile_Insert", out carrierProfileId, carrierProfile.Name, Vanrise.Common.Serializer.Serialize(carrierProfile.Settings), carrierProfile.CreatedBy, carrierProfile.LastModifiedBy);
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)carrierProfileId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(CarrierProfileToEdit carrierProfile)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierProfile_Update", carrierProfile.CarrierProfileId, carrierProfile.Name, Vanrise.Common.Serializer.Serialize(carrierProfile.Settings), carrierProfile.LastModifiedBy);
            return (recordsEffected > 0);
        }

        public bool UpdateExtendedSettings(int carrierProfileId, Dictionary<string,Object> extendedSettings)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierProfile_UpdateExtendedSettings", carrierProfileId, Vanrise.Common.Serializer.Serialize(extendedSettings));
            return (recordsEffected > 0);
        }
        public bool AreCarrierProfilesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("TOneWhS_BE.CarrierProfile", ref updateHandle);
        }
        public List<CarrierProfile> GetCarrierProfiles()
        {
            return GetItemsSP("TOneWhS_BE.sp_CarrierProfile_GetAll", CarrierProfileMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private CarrierProfile CarrierProfileMapper(IDataReader reader)
        {
            CarrierProfile carrierProfile = new CarrierProfile();
             
                carrierProfile.CarrierProfileId = (int)reader["ID"];
                carrierProfile.Name = reader["Name"] as string;
                carrierProfile.Settings = Vanrise.Common.Serializer.Deserialize<CarrierProfileSettings>(reader["Settings"] as string);
                carrierProfile.SourceId = reader["SourceId"] as string;
                carrierProfile.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
                carrierProfile.ExtendedSettings = Vanrise.Common.Serializer.Deserialize(reader["ExtendedSettings"] as string) as Dictionary<string,Object>;
                carrierProfile.IsDeleted = GetReaderValue<bool>(reader, "IsDeleted");
                carrierProfile.CreatedTime = GetReaderValue<DateTime>(reader, "CreatedTime");
                carrierProfile.CreatedBy = GetReaderValue<int>(reader, "CreatedBy");
                carrierProfile.LastModifiedBy = GetReaderValue<int>(reader, "LastModifiedBy");
                carrierProfile.LastModifiedTime = GetReaderValue<DateTime>(reader, "LastModifiedTime");
            
            return carrierProfile;
        }

        #endregion
      
    }
}
