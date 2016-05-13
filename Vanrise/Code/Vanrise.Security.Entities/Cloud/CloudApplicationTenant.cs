using System;
using System.Collections.Generic;
using Vanrise.Entities;

namespace Vanrise.Security.Entities
{
    public class CloudApplicationTenant
    {
        public Tenant Tenant { get; set; }
    }

    public class GetApplicationTenantsInput
    {
    }

    public class GetApplicationTenantsOutput
    {
        public List<CloudApplicationTenant> Tenants { get; set; }
    }

    public class CheckApplicationTenantsUpdatedInput
    {
        public object LastReceivedUpdateInfo { get; set; }
    }

    public class CheckApplicationTenantsUpdatedOuput
    {
        public bool Updated { get; set; }

        public object LastUpdateInfo { get; set; }
    }

    public class AddTenantToApplicationInput
    {
        public string Name { get; set; }

        public TenantSettings Settings { get; set; }

        public int? ParentTenantId { get; set; }
    }

    public class AddTenantToApplicationOutput
    {
        public InsertOperationOutput<CloudApplicationTenant> OperationOutput { get; set; }
    }

    public class UpdateTenantToApplicationInput
    {
        public int TenantId { get; set; }
    }

    public class UpdateTenantToApplicationOutput
    {
        public UpdateOperationOutput<CloudApplicationTenant> OperationOutput { get; set; }
    }
    
    public class CloudTenantInput
    {
    }

    public class CloudTenantOutput
    {
        public int TenantId { get; set; }
        public DateTime? LicenseExpiresOn { get; set; }
    }
}
