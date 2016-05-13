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
    public class CloudApplicationTenantDataManager : BaseSQLDataManager, ICloudApplicationTenantDataManager
    {
        public CloudApplicationTenantDataManager()
            : base(GetConnectionStringName("CloudPortal_BE_DBConnStringKey", "CloudPortal_BE_DBConnString"))
        {

        }
        public bool AreApplicationTenantsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("cloud.CloudApplicationTenant", ref updateHandle);
        }

        public List<Entities.CloudApplicationTenant> GetAllApplicationTenants()
        {
            return GetItemsSP("cloud.sp_CloudApplicationTenant_GetAll", CloudApplicationTenantMapper);
        }

        public bool AddApplicationTenant(Entities.CloudApplicationTenant applicationTenant)
        {
            return ExecuteNonQuerySP("cloud.sp_CloudApplicationTenant_Insert", applicationTenant.ApplicationId, applicationTenant.TenantId,
                applicationTenant.TenantSettings != null ? Serializer.Serialize(applicationTenant.TenantSettings) : null,
                applicationTenant.CloudTenantSettings != null ? Serializer.Serialize(applicationTenant.CloudTenantSettings) : null) > 0;
        }

        public bool UpdateApplicationTenant(CloudApplicationTenant applicationTenant)
        {
            return ExecuteNonQuerySP("cloud.sp_CloudApplicationTenant_Update", applicationTenant.ApplicationId, applicationTenant.TenantId,
                applicationTenant.TenantSettings != null ? Serializer.Serialize(applicationTenant.TenantSettings) : null,
                applicationTenant.CloudTenantSettings != null ? Serializer.Serialize(applicationTenant.CloudTenantSettings) : null) > 0;
        }

        #region Mappers

        private CloudApplicationTenant CloudApplicationTenantMapper(IDataReader reader)
        {
            var tenantSettings = reader["TenantSettings"] as string;
            var cloudTenantSettings = reader["CloudTenantSettings"] as string;
            CloudApplicationTenant appUser = new CloudApplicationTenant
            {
                CloudApplicationTenantId = (int)reader["ID"],
                ApplicationId = (int)reader["ApplicationID"],
                TenantId = (int)reader["TenantId"],
                TenantSettings = !string.IsNullOrEmpty(tenantSettings) ? Serializer.Deserialize<Vanrise.Security.Entities.TenantSettings>(tenantSettings) : null,
                CloudTenantSettings = !string.IsNullOrEmpty(cloudTenantSettings) ? Serializer.Deserialize<CloudApplicationTenantSettings>(cloudTenantSettings) : null
            };
            return appUser;
        }

        #endregion
    }
}