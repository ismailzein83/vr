using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class TenantManager
    {
        public string GetConnectionString(int tenantId, string connStringKey)
        {
            string cacheName = String.Format("TenantManager_GetConnectionString_{0}_{1}", tenantId, connStringKey);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    Tenant tenant = GetTenant(tenantId);
                    if (tenant == null)
                        throw new NullReferenceException(String.Format("tenant '{0}'", tenantId));
                    return GetConnectionString(tenant, connStringKey);
                });
            
        }

        private string GetConnectionString(Tenant tenant, string connStringKey)
        {
            if(tenant.Settings != null && tenant.Settings.ConnectionStrings != null)
            {
                var tenantConnectionString = tenant.Settings.ConnectionStrings.FirstOrDefault(itm => itm.ConnectionStringKey == connStringKey);
                if (tenantConnectionString != null)
                    return tenantConnectionString.ConnectionString;
            }
            if (tenant.ParentTenantId.HasValue)
            {
                Tenant parentTenant = GetTenant(tenant.ParentTenantId.Value);
                if (parentTenant == null)
                    throw new NullReferenceException(String.Format("tenant '{0}'", tenant.ParentTenantId.Value));
                return GetConnectionString(parentTenant, connStringKey);
            }
            return null;
        }

        private Tenant GetTenant(int tenantId)
        {
            throw new NotImplementedException();
        }

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {

        }

        #endregion
    }
}
