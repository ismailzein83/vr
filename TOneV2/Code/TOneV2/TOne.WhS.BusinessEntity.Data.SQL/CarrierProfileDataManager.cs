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

            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierProfile_Insert", out carrierProfileId, carrierProfile.Name, Vanrise.Common.Serializer.Serialize(carrierProfile.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)carrierProfileId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(CarrierProfileToEdit carrierProfile)
        {
            int recordsEffected = ExecuteNonQuerySP("TOneWhS_BE.sp_CarrierProfile_Update", carrierProfile.CarrierProfileId, carrierProfile.Name, Vanrise.Common.Serializer.Serialize(carrierProfile.Settings));
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
            CarrierProfile carrierProfile = new CarrierProfile
            {
                CarrierProfileId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<CarrierProfileSettings>(reader["Settings"] as string),
                SourceId = reader["SourceId"] as string,
                IsDeleted = GetReaderValue<bool>(reader, "IsDeleted")
            };
            return carrierProfile;
        }

        #endregion
      
    }
}
