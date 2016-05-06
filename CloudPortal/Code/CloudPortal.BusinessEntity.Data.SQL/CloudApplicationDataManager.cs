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
    public class CloudApplicationDataManager : BaseSQLDataManager, ICloudApplicationDataManager
    {
        public CloudApplicationDataManager()
            : base(GetConnectionStringName("CloudPortal_BE_DBConnStringKey", "CloudPortal_BE_DBConnString"))
        {

        }

        public bool AreCloudApplicationsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("cloud.CloudApplication", ref updateHandle);
        }

        public List<Entities.CloudApplication> GetAllCloudApplications()
        {
            return GetItemsSP("cloud.sp_CloudApplication_GetAll", CloudApplicationMapper);
        }

        public bool AddCloudApplication(CloudApplicationToAdd cloudApplicationToAdd, Vanrise.Security.Entities.CloudApplicationIdentification appIdentification, out int cloudApplicationId)
        {
            object insertedId;

            int recordesEffected = ExecuteNonQuerySP("cloud.sp_CloudApplication_Insert", out insertedId, cloudApplicationToAdd.Name, cloudApplicationToAdd.CloudApplicationTypeId,
                Vanrise.Common.Serializer.Serialize(cloudApplicationToAdd.Settings), Vanrise.Common.Serializer.Serialize(appIdentification));

            cloudApplicationId = recordesEffected > 0 ? (int)insertedId : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateCloudApplication(CloudApplicationToUpdate cloudApplicationToUpdate)
        {

            int recordesEffected = ExecuteNonQuerySP("cloud.sp_CloudApplication_Update", cloudApplicationToUpdate.CloudApplicationId, cloudApplicationToUpdate.CloudApplicationTypeId,
                Vanrise.Common.Serializer.Serialize(cloudApplicationToUpdate.Settings));

            return (recordesEffected > 0);
        }

        public void SetApplicationReady(int cloudApplicationId)
        {
            ExecuteNonQuerySP("cloud.sp_CloudApplication_SetReady", cloudApplicationId);
        }

        public void DeleteCloudApplication(int cloudApplicationId)
        {
            ExecuteNonQuerySP("cloud.sp_CloudApplication_Delete", cloudApplicationId);
        }

        #region Mappers

        private CloudApplication CloudApplicationMapper(IDataReader reader)
        {
            CloudApplication app = new CloudApplication
            {
                CloudApplicationId = (int)reader["ID"],
                Name = reader["Name"] as string,
                CloudApplicationTypeId = (int)reader["CloudApplicationTypeId"],
                ApplicationIdentification = Serializer.Deserialize<Vanrise.Security.Entities.CloudApplicationIdentification>(reader["ApplicationIdentification"] as string),
                Settings = Serializer.Deserialize<CloudApplicationSettings>(reader["Settings"] as string)
            };
            return app;
        }

        #endregion
    }
}
