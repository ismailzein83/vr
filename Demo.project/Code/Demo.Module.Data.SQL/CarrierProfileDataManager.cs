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
    public class OperatorProfileDataManager : BaseSQLDataManager, IOperatorProfileDataManager
    {
   
        #region ctor/Local Variables
        public OperatorProfileDataManager()
            : base(GetConnectionStringName("DemoProject_DBConnStringKey", "DemoDBConnectionString"))
        {

        }
        #endregion

        #region Public Methods
        public bool Insert(OperatorProfile operatorProfile, out int insertedId)
        {
            object operatorProfileId;

            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorProfile_Insert", out operatorProfileId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings));
            bool insertedSuccesfully = (recordsEffected > 0);
            if (insertedSuccesfully)
                insertedId = (int)operatorProfileId;
            else
                insertedId = 0;
            return insertedSuccesfully;
        }
        public bool Update(OperatorProfile operatorProfile)
        {
            int recordsEffected = ExecuteNonQuerySP("dbo.sp_OperatorProfile_Update", operatorProfile.OperatorProfileId, operatorProfile.Name, Vanrise.Common.Serializer.Serialize(operatorProfile.Settings));
            return (recordsEffected > 0);
        }
        public bool AreOperatorProfilesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("dbo.OperatorProfile", ref updateHandle);
        }
        public List<OperatorProfile> GetOperatorProfiles()
        {
            return GetItemsSP("dbo.sp_OperatorProfile_GetAll", OperatorProfileMapper);
        }
        #endregion

        #region Private Methods

        #endregion

        #region  Mappers
        private OperatorProfile OperatorProfileMapper(IDataReader reader)
        {
            OperatorProfile operatorProfile = new OperatorProfile
            {
                OperatorProfileId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<OperatorProfileSettings>(reader["Settings"] as string)
            };
            return operatorProfile;
        }

        #endregion
      
    }
}
