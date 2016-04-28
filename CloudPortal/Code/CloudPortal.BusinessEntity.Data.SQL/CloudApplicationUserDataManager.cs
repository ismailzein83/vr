using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace CloudPortal.BusinessEntity.Data.SQL
{
    public class CloudApplicationUserDataManager : BaseSQLDataManager, ICloudApplicationUserDataManager
    {
        public CloudApplicationUserDataManager()
            : base(GetConnectionStringName("CloudPortal_BE_DBConnStringKey", "CloudPortal_BE_DBConnString"))
        {

        }
        public bool AreApplicationUsersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("cloud.CloudApplicationUser", ref updateHandle);
        }

        public List<Entities.CloudApplicationUser> GetAllApplicationUsers()
        {
            return GetItemsSP("cloud.sp_CloudApplicationUser_GetAll", CloudApplicationUserMapper);
        }

        public bool AddApplicationUser(Entities.CloudApplicationUser applicationUser)
        {
            return ExecuteNonQuerySP("cloud.sp_CloudApplicationUser_Insert", applicationUser.ApplicationId, applicationUser.UserId, 
                applicationUser.Settings != null ? Serializer.Serialize(applicationUser.Settings) : null) > 0;
        }

        #region Mappers

        private CloudApplicationUser CloudApplicationUserMapper(IDataReader reader)
        {
            CloudApplicationUser appUser = new CloudApplicationUser
            {
                ApplicationId = (int)reader["ApplicationID"],
                UserId = (int)reader["UserID"],
                Settings = Serializer.Deserialize<CloudApplicationUserSettings>(reader["Settings"] as string)
            };
            return appUser;
        }

        #endregion
    }
}
