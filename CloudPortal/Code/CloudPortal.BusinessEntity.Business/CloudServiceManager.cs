using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Business;
using VRSecEntities = Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudServiceManager
    {
        static TimeSpan s_resetPasswordValidity;

        static CloudServiceManager()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["TempPasswordValidity"], out s_resetPasswordValidity))
                s_resetPasswordValidity = new TimeSpan(1, 0, 0);
        }


        #region public methods

        public List<VRSecEntities.CloudApplicationTenant> GetApplicationTenants(VRSecEntities.CloudApplicationIdentification applicationIdentification)
        {
            CloudApplicationTenantManager appTenantManager = new CloudApplicationTenantManager();
            List<CloudApplicationTenant> appTenants = appTenantManager.GetApplicationTenants(applicationIdentification);
            if (appTenants == null)
                appTenants = new List<CloudApplicationTenant>();

            List<Vanrise.Security.Entities.CloudApplicationTenant> applicationTenants = new List<VRSecEntities.CloudApplicationTenant>();
            TenantManager tenantManager = new TenantManager();
            foreach (var appTenant in appTenants)
            {
                var tenant = tenantManager.GetTenantbyId(appTenant.TenantId);
                if (tenant == null)
                    throw new NullReferenceException(String.Format("tenant. ID '{0}'", appTenant.TenantId));
                var cloudAppUser = MapTenantToAppTenant(tenant);
                applicationTenants.Add(cloudAppUser);
            }
            return applicationTenants;
        }

        public InsertOperationOutput<VRSecEntities.CloudApplicationTenant> AddTenantToApplication(VRSecEntities.CloudApplicationIdentification applicationIdentification, string name, int? parentTenantId, VRSecEntities.TenantSettings tenantSettings)
        {
            TenantManager tenantManager = new TenantManager();
            var tenant = tenantManager.GetTenantbyName(name);
            if (tenant == null)
            {
                var addTenantOutput = tenantManager.AddTenant(new VRSecEntities.Tenant
                {
                    Name = name,
                    ParentTenantId = parentTenantId,
                    Settings = tenantSettings
                });
                if (addTenantOutput == null || addTenantOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded)
                {
                    if (addTenantOutput.InsertedObject != null)
                        tenant = addTenantOutput.InsertedObject.Entity;
                }
                if (tenant == null)
                    throw new Exception(String.Format("Could not insert Tenant '{0}'", name));
            }
            CloudApplicationTenantManager appTenantManager = new CloudApplicationTenantManager();
            CloudApplicationTenant appTenant;
            bool isAdded = appTenantManager.AddApplicationTenant(applicationIdentification, tenant.TenantId, out appTenant);
            if (isAdded)
                return new InsertOperationOutput<VRSecEntities.CloudApplicationTenant>
                {
                    Result = InsertOperationResult.Succeeded,
                    InsertedObject = MapTenantToAppTenant(tenant)
                };
            else
                return new InsertOperationOutput<VRSecEntities.CloudApplicationTenant>
                {
                    Result = InsertOperationResult.SameExists
                };
        }

        public UpdateOperationOutput<VRSecEntities.CloudApplicationTenant> UpdateTenantToApplication(VRSecEntities.CloudApplicationIdentification applicationIdentification, int tenantId)
        {
            TenantManager tenantManager = new TenantManager();
            var tenant = tenantManager.GetTenantbyId(tenantId);
            if (tenant == null)
            {
                throw new Exception(String.Format("Could not Update Tenant '{0}'", tenantId));
            }
            CloudApplicationTenantManager appTenantManager = new CloudApplicationTenantManager();
            CloudApplicationTenant appTenant;
            bool isupdated = appTenantManager.UpdateApplicationTenant(applicationIdentification, tenant.TenantId, out appTenant);
            if (isupdated)
                return new UpdateOperationOutput<VRSecEntities.CloudApplicationTenant>
                {
                    Result = UpdateOperationResult.Succeeded,
                    UpdatedObject = MapTenantToAppTenant(tenant)
                };
            else
                return new UpdateOperationOutput<VRSecEntities.CloudApplicationTenant>
                {
                    Result = UpdateOperationResult.Failed
                };
        }

        public bool CheckAppTenantsUpdated(ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<TenantCacheManager>().IsCacheExpired(ref lastCheckTime);
        }


        public List<VRSecEntities.CloudTenantOutput> GetCloudTenants(VRSecEntities.CloudApplicationIdentification applicationIdentification)
        {
            CloudApplicationTenantManager cloudApplicationTenantManager = new CloudApplicationTenantManager();
            var items = cloudApplicationTenantManager.GetApplicationTenants(applicationIdentification);
            if (items == null || items.Count == 0)
                return null;

            List<VRSecEntities.CloudTenantOutput> results = new List<VRSecEntities.CloudTenantOutput>();
            foreach (CloudApplicationTenant item in items)
            {
                results.Add(new VRSecEntities.CloudTenantOutput()
                {
                    TenantId = item.TenantId,
                    LicenseExpiresOn = item.CloudTenantSettings != null ? item.CloudTenantSettings.LicenseExpiresOn : null
                });
            }

            return results;
        }


        public List<Vanrise.Security.Entities.CloudApplicationUser> GetApplicationUsers(VRSecEntities.CloudApplicationIdentification applicationIdentification)
        {
            CloudApplicationUserManager appUserManager = new CloudApplicationUserManager();
            List<CloudApplicationUser> appUsers = appUserManager.GetApplicationUsers(applicationIdentification);
            if (appUsers == null)
                appUsers = new List<CloudApplicationUser>();

            List<Vanrise.Security.Entities.CloudApplicationUser> applicationUsers = new List<VRSecEntities.CloudApplicationUser>();
            UserManager userManager = new UserManager();
            foreach (var appUser in appUsers)
            {
                var user = userManager.GetUserbyId(appUser.UserId);
                if (user == null)
                    throw new NullReferenceException(String.Format("user. ID '{0}'", appUser.UserId));
                var cloudAppUser = MapUserToAppUser(user, appUser);
                applicationUsers.Add(cloudAppUser);
            }
            return applicationUsers;
        }

        public InsertOperationOutput<VRSecEntities.CloudApplicationUser> AddUserToApplication(VRSecEntities.CloudApplicationIdentification applicationIdentification, string userEmail, VRSecEntities.UserStatus appUserStatus, string appUserDescription, int tenantId)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyEmail(userEmail);
            if (user == null)
            {
                var addUserOutput = userManager.AddUser(new VRSecEntities.UserToAdd
                    {
                        Email = userEmail,
                        Name = userEmail,
                        Description = appUserDescription,
                        TenantId = tenantId
                    });
                if (addUserOutput == null || addUserOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded)
                {
                    if (addUserOutput.InsertedObject != null)
                        user = addUserOutput.InsertedObject.Entity;
                }
                if (user == null)
                    throw new Exception(String.Format("Could not insert User '{0}'", userEmail));
            }

            if (user.TenantId != tenantId)
            {
                return new InsertOperationOutput<VRSecEntities.CloudApplicationUser>
                {
                    Result = InsertOperationResult.Failed,
                    Message="Same user exists under another company",
                    ShowExactMessage = true
                };
            }
            else
            {
                CloudApplicationUserManager appUserManager = new CloudApplicationUserManager();
                CloudApplicationUser appUser;
                bool isAdded = appUserManager.AddApplicationUser(applicationIdentification, user.UserId, appUserStatus, appUserDescription, tenantId, out appUser);
                if (isAdded)
                    return new InsertOperationOutput<VRSecEntities.CloudApplicationUser>
                    {
                        Result = InsertOperationResult.Succeeded,
                        InsertedObject = MapUserToAppUser(user, appUser)
                    };
                else
                    return new InsertOperationOutput<VRSecEntities.CloudApplicationUser>
                    {
                        Result = InsertOperationResult.SameExists
                    };
            }
        }

        public UpdateOperationOutput<VRSecEntities.CloudApplicationUser> UpdateUserToApplication(VRSecEntities.CloudApplicationIdentification applicationIdentification, int userId, VRSecEntities.UserStatus appUserStatus, string appUserDescription)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyId(userId);
            if (user == null)
            {
                throw new Exception(String.Format("Could not Update User '{0}'", userId));
            }
            CloudApplicationUserManager appUserManager = new CloudApplicationUserManager();
            CloudApplicationUser appUser;
            bool isupdated = appUserManager.UpdateApplicationUser(applicationIdentification, user.UserId, appUserStatus, appUserDescription, user.TenantId, out appUser);
            if (isupdated)
                return new UpdateOperationOutput<VRSecEntities.CloudApplicationUser>
                {
                    Result = UpdateOperationResult.Succeeded,
                    UpdatedObject = MapUserToAppUser(user, appUser)
                };
            else
                return new UpdateOperationOutput<VRSecEntities.CloudApplicationUser>
                {
                    Result = UpdateOperationResult.Failed
                };
        }

        public UpdateOperationOutput<object> ResetUserPasswordApplication(int userId)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyId(userId);
            if (user == null)
                return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

            PasswordGenerator pwdGenerator = new PasswordGenerator();
            string pwd = pwdGenerator.Generate();


            UpdateOperationOutput<object> result = userManager.UpdateTempPasswordById(userId, pwd, DateTime.Now.Add(s_resetPasswordValidity));
            if (result.Result == UpdateOperationResult.Succeeded)
            {
                EmailTemplateManager emailTemplateManager = new EmailTemplateManager();
                EmailTemplate template = emailTemplateManager.GeEmailTemplateByType(Vanrise.Security.Business.Constants.ResetPasswordType);
                PasswordEmailContext context = new PasswordEmailContext() { Name = user.Name, Password = pwd };

                emailTemplateManager.SendEmail(user.Email, template.GetParsedBodyTemplate(context), template.GetParsedSubjectTemplate(context));
            }
            return result;
        }

        public UpdateOperationOutput<object> ForgotUserPasswordApplication(string email)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyEmail(email);
            if (user == null)
                return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

            PasswordGenerator pwdGenerator = new PasswordGenerator();
            string pwd = pwdGenerator.Generate();

            UpdateOperationOutput<object> result = userManager.UpdateTempPasswordByEmail(email, pwd, DateTime.Now.Add(s_resetPasswordValidity));
            if (result.Result == UpdateOperationResult.Succeeded)
            {
                EmailTemplateManager emailTemplateManager = new EmailTemplateManager();
                EmailTemplate template = emailTemplateManager.GeEmailTemplateByType(Vanrise.Security.Business.Constants.ForgotPasswordType);
                PasswordEmailContext context = new PasswordEmailContext() { Name = user.Name, Password = pwd };

                emailTemplateManager.SendEmail(email, template.GetParsedBodyTemplate(context), template.GetParsedSubjectTemplate(context));
            }
            return result;
        }

        public bool CheckAppUsersUpdated(ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<UserCacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        #endregion

        #region Private Methods

        private VRSecEntities.CloudApplicationTenant MapTenantToAppTenant(VRSecEntities.Tenant tenant)
        {
            var cloudAppTenant = new VRSecEntities.CloudApplicationTenant
            {
                Tenant = tenant
            };

            return cloudAppTenant;
        }

        private VRSecEntities.CloudApplicationUser MapUserToAppUser(VRSecEntities.User user, CloudApplicationUser appUser)
        {
            var cloudAppUser = new VRSecEntities.CloudApplicationUser
            {
                User = user
            };
            if (appUser.Settings != null)
            {
                //cloudAppUser.Status = appUser.Settings.Status;
                cloudAppUser.Description = appUser.Settings.Description;
            }
            return cloudAppUser;
        }

        #endregion

        #region Private Classes

        private class TenantCacheManager : Vanrise.Caching.BaseCacheManager
        {

            DateTime? _tenantCacheLastCheck;
            DateTime? _applicationTenantCacheLastCheck;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<TenantManager.CacheManager>().IsCacheExpired(ref _tenantCacheLastCheck)
                |
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CloudApplicationTenantManager.CacheManager>().IsCacheExpired(ref _applicationTenantCacheLastCheck);
            }
        }

        private class UserCacheManager : Vanrise.Caching.BaseCacheManager
        {

            DateTime? _userCacheLastCheck;
            DateTime? _applicationUserCacheLastCheck;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<UserManager.CacheManager>().IsCacheExpired(ref _userCacheLastCheck)
                |
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CloudApplicationUserManager.CacheManager>().IsCacheExpired(ref _applicationUserCacheLastCheck);
            }
        }

        #endregion
    }
}
