using CloudPortal.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Business;
using VRSecEntities = Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Business
{
    public class CloudServiceManager
    {
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

        public InsertOperationOutput<VRSecEntities.CloudApplicationUser> AddUserToApplication(VRSecEntities.CloudApplicationIdentification applicationIdentification, string userEmail, VRSecEntities.UserStatus appUserStatus, string appUserDescription)
        {
            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyEmail(userEmail);
            if (user == null)
            {
                var addUserOutput = userManager.AddUser(new VRSecEntities.User
                    {
                        Email = userEmail,
                        Name = userEmail,
                        Description = appUserDescription,
                        Status = VRSecEntities.UserStatus.Active
                    });
                if (addUserOutput == null || addUserOutput.Result == Vanrise.Entities.InsertOperationResult.Succeeded)
                {
                    if (addUserOutput.InsertedObject != null)
                        user = addUserOutput.InsertedObject.Entity;
                }
                if (user == null)
                    throw new Exception(String.Format("Could not insert User '{0}'", userEmail));
            }
            CloudApplicationUserManager appUserManager = new CloudApplicationUserManager();
            CloudApplicationUser appUser;
            bool isAdded = appUserManager.AddApplicationUser(applicationIdentification, user.UserId, appUserStatus, appUserDescription, out appUser);
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

        public bool CheckAppUsersUpdated(ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<UserCacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        #region Private Methods

        private VRSecEntities.CloudApplicationUser MapUserToAppUser(VRSecEntities.User user, CloudApplicationUser appUser)
        {
            var cloudAppUser = new VRSecEntities.CloudApplicationUser
            {
                User = user
            };
            if (appUser.Settings != null)
            {
                cloudAppUser.Status = appUser.Settings.Status;
                cloudAppUser.Description = appUser.Settings.Description;
            }
            return cloudAppUser;
        }

        #endregion

        #region Private Classes

        private  class UserCacheManager : Vanrise.Caching.BaseCacheManager
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
