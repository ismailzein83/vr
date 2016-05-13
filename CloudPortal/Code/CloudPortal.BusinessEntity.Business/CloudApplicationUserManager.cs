using CloudPortal.BusinessEntity.Data;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Business;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudApplicationUserManager
    {
        public Vanrise.Entities.IDataRetrievalResult<CloudApplicationUserDetail> GetFilteredCloudApplicationUsers(Vanrise.Entities.DataRetrievalInput<CloudApplicationUserQuery> input)
        {
            List<CloudApplicationUser> items = GetApplicationUsersByCloudApplicationTenantId(input.Query.CloudApplicationTenantId);

            Func<CloudApplicationUser, bool> filterExpression = (prod) => true;

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, items.ToBigResult(input, filterExpression, CloudApplicationUserDetailMapper));
        }

        public Vanrise.Entities.InsertOperationOutput<CloudApplicationUserToAssign> AssignCloudApplicationUsers(CloudApplicationUserToAssign cloudApplicationUser, bool withFullPermission)
        {
            InsertOperationOutput<CloudApplicationUserToAssign> insertOperationOutput = new InsertOperationOutput<CloudApplicationUserToAssign>();

            insertOperationOutput.Result = InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            bool insertActionSucc = true;

            ICloudApplicationUserDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();

            CloudApplicationTenantManager cloudApplicationTenantManager = new CloudApplicationTenantManager();
            var cloudApplicationTenant = cloudApplicationTenantManager.GetApplicationTenantById(cloudApplicationUser.CloudApplicationTenantID);

            List<int> insertedUsers = new List<int>();

            UserManager userManager = new UserManager();
            foreach (int userId in cloudApplicationUser.UserIds)
            {
                Vanrise.Security.Entities.User user = userManager.GetUserbyId(userId);
                CloudApplicationUser currentUser = new CloudApplicationUser()
                {
                    CloudApplicationTenantID = cloudApplicationUser.CloudApplicationTenantID,
                    UserId = userId,
                    Settings = new CloudApplicationUserSettings()
                    {
                        Status = Vanrise.Security.Entities.UserStatus.Active,
                        Description = user.Description
                    }
                };
                if (!dataManager.AddApplicationUser(currentUser))
                    insertActionSucc = false;
                else if (withFullPermission)
                {
                    insertedUsers.Add(userId);
                }
            }

            if (withFullPermission && insertedUsers.Count > 0)
            {
                AssignUserFullControlToApp(cloudApplicationTenant.ApplicationId, insertedUsers);
            }

            if (insertActionSucc)
            {
                Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = InsertOperationResult.Succeeded;
                insertOperationOutput.InsertedObject = cloudApplicationUser;
            }
            else
            {
                insertOperationOutput.Result = InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }


        public List<CloudApplicationUser> GetApplicationUsers(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification)
        {
            var application = GetApplication(applicationIdentification);
            return GetApplicationUsersByApplicationId(application.CloudApplicationId);
        }

        private static CloudApplication GetApplication(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification)
        {
            if (applicationIdentification == null)
                throw new ArgumentNullException("applicationIdentification");
            CloudApplicationManager cloudApplicationManager = new CloudApplicationManager();
            var application = cloudApplicationManager.GetApplicationByIdentification(applicationIdentification);
            if (application == null)
                throw new NullReferenceException(String.Format("application. IdentificationKey '{0}'", applicationIdentification.IdentificationKey));
            return application;
        }

        public List<CloudApplicationUser> GetUserApplications(int userId)
        {
            return GetAllAppUsersByUserId().GetRecord(userId);
        }

        public bool AddApplicationUser(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification, int userId,
            Vanrise.Security.Entities.UserStatus status, string description, int tenantId, out CloudApplicationUser appUser)
        {
            var application = GetApplication(applicationIdentification);
            var cloudApplicationTenantManager = new CloudApplicationTenantManager();
            var cloudApplicationTenant = cloudApplicationTenantManager.GetApplicationTenantByAppIdTenantId(application.CloudApplicationId, tenantId);

            appUser = new CloudApplicationUser
            {
                UserId = userId,
                CloudApplicationTenantID = cloudApplicationTenant.CloudApplicationTenantId,
                Settings = new CloudApplicationUserSettings
                {
                    Status = status,
                    Description = description
                }
            };
            ICloudApplicationUserDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();
            return dataManager.AddApplicationUser(appUser);
        }

        public bool UpdateApplicationUser(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification, int userId,
            Vanrise.Security.Entities.UserStatus status, string description, int tenantId, out CloudApplicationUser appUser)
        {
            var application = GetApplication(applicationIdentification);
            var cloudApplicationTenantManager = new CloudApplicationTenantManager();
            var cloudApplicationTenant = cloudApplicationTenantManager.GetApplicationTenantByAppIdTenantId(application.CloudApplicationId, tenantId);

            appUser = new CloudApplicationUser
            {
                UserId = userId,
                CloudApplicationTenantID = cloudApplicationTenant.CloudApplicationTenantId,
                Settings = new CloudApplicationUserSettings
                {
                    Status = status,
                    Description = description
                }
            };
            ICloudApplicationUserDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();
            return dataManager.UpdateApplicationUser(appUser);
        }

        public List<CloudApplicationUser> GetApplicationUsersByApplicationId(int applicationId)
        {
            return GetAllAppUsersByAppId().GetRecord(applicationId);
        }

        #region Private Methods
        private void AssignUserFullControlToApp(int applicationId, List<int> userIds)
        {
            CloudApplicationManager cloudApplpicationManager = new CloudApplicationManager();
            var application = cloudApplpicationManager.GetCloudApplication(applicationId);
            CloudApplicationServiceProxy appServiceProxy = new CloudApplicationServiceProxy(application);
            var assignUserFullControlInput = new Vanrise.Security.Entities.AssignUserFullControlInput
            {
                UserIds = userIds
            };
            var assignUserFullControlOutput = appServiceProxy.AssignUserFullControl(assignUserFullControlInput);
        }

        private List<CloudApplicationUser> GetApplicationUsersByCloudApplicationTenantId(int cloudApplicationTenantId)
        {
            return GetAllAppUsersByAppTenantId().GetRecord(cloudApplicationTenantId);
        }



        private Dictionary<int, List<CloudApplicationUser>> GetAllAppUsersByAppTenantId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllAppUsersByAppTenantId",
                () =>
                {
                    ICloudApplicationUserDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();
                    List<CloudApplicationUser> allApplicationUsers = dataManager.GetAllApplicationUsers();
                    if (allApplicationUsers == null)
                        return null;
                    else
                    {
                        Dictionary<int, List<CloudApplicationUser>> dicAppUsers = new Dictionary<int, List<CloudApplicationUser>>();
                        foreach (var appUser in allApplicationUsers)
                        {
                            dicAppUsers.GetOrCreateItem(appUser.CloudApplicationTenantID).Add(appUser);
                        }
                        return dicAppUsers;
                    }
                });
        }

        private Dictionary<int, List<CloudApplicationUser>> GetAllAppUsersByAppId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllAppUsersByAppId",
                () =>
                {
                    ICloudApplicationUserDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();
                    List<CloudApplicationUser> allApplicationUsers = dataManager.GetAllApplicationUsers();
                    if (allApplicationUsers == null)
                        return null;
                    else
                    {
                        var appTenantManager = new CloudApplicationTenantManager();
                        Dictionary<int, List<CloudApplicationUser>> dicAppUsers = new Dictionary<int, List<CloudApplicationUser>>();
                        foreach (var appUser in allApplicationUsers)
                        {
                            var cloudApplicationTenant = appTenantManager.GetApplicationTenantById(appUser.CloudApplicationTenantID);
                            dicAppUsers.GetOrCreateItem(cloudApplicationTenant.ApplicationId).Add(appUser);
                        }
                        return dicAppUsers;
                    }
                });
        }

        private Dictionary<int, List<CloudApplicationUser>> GetAllAppUsersByUserId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllAppUsersByUserId",
                () =>
                {
                    ICloudApplicationUserDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();
                    List<CloudApplicationUser> allApplicationUsers = dataManager.GetAllApplicationUsers();
                    if (allApplicationUsers == null)
                        return null;
                    else
                    {

                        Dictionary<int, List<CloudApplicationUser>> dicAppUsers = new Dictionary<int, List<CloudApplicationUser>>();
                        foreach (var appUser in allApplicationUsers)
                        {
                            dicAppUsers.GetOrCreateItem(appUser.UserId).Add(appUser);
                        }
                        return dicAppUsers;
                    }
                });
        }

        private CloudApplicationUserDetail CloudApplicationUserDetailMapper(CloudApplicationUser cloudApplicationUser)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyId(cloudApplicationUser.UserId);

            return new CloudApplicationUserDetail()
            {
                Entity = cloudApplicationUser,
                UserName = user.Name
            };
        }

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICloudApplicationUserDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreApplicationUsersUpdated(ref _updateHandle);
            }
        }

        #endregion
    }
}
