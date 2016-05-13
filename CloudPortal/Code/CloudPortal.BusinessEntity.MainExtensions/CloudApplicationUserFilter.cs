using CloudPortal.BusinessEntity.Business;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudPortal.BusinessEntity.MainExtensions
{
    public class CloudApplicationUserFilter : Vanrise.Security.Entities.IUserFilter
    {
        public int CloudApplicationTenantId { get; set; }
        public bool IsExcluded(Vanrise.Security.Entities.User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            CloudApplicationTenantManager cloudApplicationTenantManager = new CloudApplicationTenantManager();
            CloudApplicationTenant cloudApplicationTenant =  cloudApplicationTenantManager.GetApplicationTenantById(CloudApplicationTenantId);

            CloudApplicationUserManager cloudApplicationUserManager = new CloudApplicationUserManager();
            List<CloudApplicationUser> cloudApplicationUsers = cloudApplicationUserManager.GetApplicationUsersByApplicationId(cloudApplicationTenant.ApplicationId);

            if (cloudApplicationUsers == null || cloudApplicationUsers.Count == 0)
                return false;

            if (cloudApplicationUsers.FirstOrDefault(itm => itm.UserId == user.UserId) != null)
                return true;

            return false;
        }
    }
}