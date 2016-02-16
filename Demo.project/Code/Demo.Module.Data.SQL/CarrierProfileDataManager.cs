using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.Module.Entities;
using Vanrise.Data.SQL;

namespace Demo.Module.Data.SQL
{
    public class CarrierProfileDataManager : BaseSQLDataManager, ICarrierProfileDataManager
    {
   
        #region ctor/Local Variables
        public CarrierProfileDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(CarrierProfile carrierProfile, out int insertedId)
        {
            object carrierProfileId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_CarrierProfile_Insert", out carrierProfileId, carrierProfile.Name, Vanrise.Common.Serializer.Serialize(carrierProfile.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)carrierProfileId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(CarrierProfile carrierProfile)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_CarrierProfile_Update", carrierProfile.CarrierProfileId, carrierProfile.Name, Vanrise.Common.Serializer.Serialize(carrierProfile.Settings));
            return (recordsEffected > 0);
        }
        public bool AreCarrierProfilesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.CarrierProfile", ref updateHandle);
        }
        public List<CarrierProfile> GetCarrierProfiles()
        {
            return GetItemsSP("dbo.sp_CarrierProfile_GetAll", CarrierProfileMapper);
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
                Settings = Vanrise.Common.Serializer.Deserialize<CarrierProfileSettings>(reader["Settings"] as string)
            };
            return carrierProfile;
        }

        #endregion
      
    }
}
