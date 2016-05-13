using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudPortal.BusinessEntity.Data
{
    public interface ICloudApplicationTenantDataManager : IDataManager
    {
        bool AreApplicationTenantsUpdated(ref object updateHandle);

        List<Entities.CloudApplicationTenant> GetAllApplicationTenants();

        bool AddApplicationTenant(Entities.CloudApplicationTenant applicationTenant);

        bool UpdateApplicationTenant(Entities.CloudApplicationTenant applicationTenant);
    }
}
