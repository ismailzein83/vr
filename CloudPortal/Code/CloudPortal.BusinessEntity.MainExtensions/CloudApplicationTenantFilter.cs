using CloudPortal.BusinessEntity.Business;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudPortal.BusinessEntity.MainExtensions
{
    public class CloudApplicationTenantFilter : Vanrise.Security.Entities.ITenantFilter
    {
        public int ApplicationId { get; set; }
        public bool IsExcluded(Vanrise.Security.Entities.Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException("tenant");

            CloudApplicationTenantManager manager = new CloudApplicationTenantManager();
            List<CloudApplicationTenant> assignedTenants = manager.GetApplicationTenants(ApplicationId);
            if (assignedTenants == null || assignedTenants.Count == 0)
                return false;

            if (assignedTenants.FirstOrDefault(itm => itm.TenantId == tenant.TenantId) != null)
                return true;

            return false;
        }
    }
}