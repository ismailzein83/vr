using CloudPortal.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;

namespace CloudPortal.BusinessEntity.Data.SQL
{
    public class CloudApplicationTypeDataManager : BaseSQLDataManager, ICloudApplicationTypeDataManager
    {
        public CloudApplicationTypeDataManager()
            : base(GetConnectionStringName("CloudPortal_BE_DBConnStringKey", "CloudPortal_BE_DBConnString"))
        {

        }

        public bool AreCloudApplicationTypesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("cloud.CloudApplicationType", ref updateHandle);
        }

        public List<CloudApplicationType> GetAllCloudApplicationTypes()
        {
            return GetItemsSP("cloud.sp_CloudApplicationType_GetAll", CloudApplicationTypeMapper);
        }

        public bool AddCloudApplicationType(CloudApplicationType cloudApplicationToAdd, out int applicationTypeId)
        {
            object insertedId;

            int recordesEffected = ExecuteNonQuerySP("cloud.sp_CloudApplicationType_Insert", out insertedId, cloudApplicationToAdd.Name);
            applicationTypeId = recordesEffected > 0 ? (int)insertedId : -1;
            return (recordesEffected > 0);
        }

        public bool UpdateCloudApplicationType(CloudApplicationType cloudApplicationToUpdate)
        {
            int recordesEffected = ExecuteNonQuerySP("cloud.sp_CloudApplicationType_Update", cloudApplicationToUpdate.CloudApplicationTypeId, cloudApplicationToUpdate.Name);
            return (recordesEffected > 0);
        }

        #region Mappers

        private CloudApplicationType CloudApplicationTypeMapper(IDataReader reader)
        {
            CloudApplicationType type = new CloudApplicationType
            {
                CloudApplicationTypeId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            return type;
        }

        #endregion
    }
}