using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data.SQL
{
    public class TenantDataManager : BaseSQLDataManager, ITenantDataManager
    {
        #region ctor
        public TenantDataManager()
            : base(GetConnectionStringName("SecurityDBConnStringKey", "SecurityDBConnString"))
        {

        }

        #endregion

        #region Public Methods


        public List<Tenant> GetTenants()
        {
            return GetItemsSP("sec.sp_Tenant_GetAll", TenantMapper);
        }

        public bool AddTenant(Tenant tenantObject, out int insertedId)
        {
            object tenantID;

            int recordesEffected = ExecuteNonQuerySP("sec.sp_Tenant_Insert", out tenantID, tenantObject.Name,
                tenantObject.Settings != null ? Vanrise.Common.Serializer.Serialize(tenantObject.Settings) : null, tenantObject.ParentTenantId);
            insertedId = (recordesEffected > 0) ? (int)tenantID : -1;

            return (recordesEffected > 0);
        }

        public bool UpdateTenant(Tenant tenantObject)
        {
            int recordesEffected = ExecuteNonQuerySP("sec.sp_Tenant_Update", tenantObject.TenantId, tenantObject.Name,
                tenantObject.Settings != null ? Vanrise.Common.Serializer.Serialize(tenantObject.Settings) : null, tenantObject.ParentTenantId);
            return (recordesEffected > 0);
        }

        public bool AreTenantsUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("sec.[Tenant]", ref updateHandle);
        }

        #endregion

        #region Mappers

        private Tenant TenantMapper(IDataReader reader)
        {
            var settings = reader["Settings"] as string;
            return new Entities.Tenant
            {
                TenantId = Convert.ToInt32(reader["Id"]),
                ParentTenantId = GetReaderValue<int?>(reader, "ParentTenantId"),
                Name = reader["Name"] as string,
                Settings = !string.IsNullOrEmpty(settings) ? Vanrise.Common.Serializer.Deserialize<TenantSettings>(settings) : null
            };
        }

        #endregion
    }
}