using CloudPortal.BusinessEntity.Data;
using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudApplicationUserManager
    {
        public List<CloudApplicationUser> GetApplicationUsers(Vanrise.Security.Entities.CloudApplicationIdentification applicationIdentification)
        {
            var application = GetApplication(applicationIdentification);
            return GetApplicationUsers(application.CloudApplicationId);
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
            Vanrise.Security.Entities.UserStatus status, string description, out CloudApplicationUser appUser)
        {
            var application = GetApplication(applicationIdentification);
            appUser = new CloudApplicationUser
            {
                UserId = userId,
                ApplicationId = application.CloudApplicationId,
                Settings = new CloudApplicationUserSettings
                {
                    Status = status,
                    Description = description
                }
            };
            ICloudApplicationUserDataManager dataManager = BEDataManagerFactory.GetDataManager<ICloudApplicationUserDataManager>();
            return dataManager.AddApplicationUser(appUser);
        }

        #region Private Methods

        private List<CloudApplicationUser> GetApplicationUsers(int applicationId)
        {
            return GetAllAppUsersByAppId().GetRecord(applicationId);
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

                        Dictionary<int, List<CloudApplicationUser>> dicAppUsers = new Dictionary<int, List<CloudApplicationUser>>();
                        foreach (var appUser in allApplicationUsers)
                        {
                            dicAppUsers.GetOrCreateItem(appUser.ApplicationId).Add(appUser);
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
