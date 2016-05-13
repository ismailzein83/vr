using System;
using Vanrise.Security.Entities;
namespace CloudPortal.BusinessEntity.Entities
{
    public class CloudApplicationTenant
    {
        public int CloudApplicationTenantId { get; set; }

        public int ApplicationId { get; set; }

        public int TenantId { get; set; }

        public TenantSettings TenantSettings { get; set; }
        
        public CloudApplicationTenantSettings CloudTenantSettings { get; set; }
    }

    public class CloudApplicationTenantSettings
    {
        public DateTime? LicenseExpiresOn { get; set; }
    }
}