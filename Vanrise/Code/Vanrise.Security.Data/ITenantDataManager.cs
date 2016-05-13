using System.Collections.Generic;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Data
{
    public interface ITenantDataManager : IDataManager
    {
        List<Tenant> GetTenants();

        bool AddTenant(Tenant tenant, out int insertedId);

        bool UpdateTenant(Tenant tenant);

        bool AreTenantsUpdated(ref object updateHandle);
    }
}
