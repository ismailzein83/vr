using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Caching;
using Vanrise.Common;

namespace Vanrise.Security.Business
{
    public class UserManager
    {
        #region Public Methods

        public IDataRetrievalResult<UserDetail> GetFilteredUsers(DataRetrievalInput<UserQuery> input)
        {
            var allItems = GetCachedUsers();

            Func<User, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.Email == null || itemObject.Email.ToLower().Contains(input.Query.Email.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, UserDetailMapper));
        }

        public IEnumerable<User> GetUsers()
        {
            var users = GetCachedUsers();
            return users.Values;
        }

        public IEnumerable<UserInfo> GetUsersInfo(UserFilter filter)
        {
            var users = GetCachedUsers();

            if (filter != null)
            {
                if (filter.EntityType != null && filter.EntityId != null)
                {
                    PermissionManager permissionManager = new PermissionManager();
                    IEnumerable<Permission> entityPermissions = permissionManager.GetEntityPermissions((EntityType)filter.EntityType, filter.EntityId);

                    IEnumerable<int> excludedUserIds = entityPermissions.MapRecords(permission => Convert.ToInt32(permission.HolderId), permission => permission.HolderType == HolderType.USER);
                    return users.MapRecords(UserInfoMapper, user => !excludedUserIds.Contains(user.UserId) || (filter.ExcludeInactive == true && user.Status == UserStatus.Active));
                }

            }
            return users.MapRecords(UserInfoMapper, user => (filter == null || (filter.ExcludeInactive == true && user.Status == UserStatus.Active)));
        }

        public User GetUserbyId(int userId)
        {
            var users = GetCachedUsers(); 
            return users.GetRecord(userId);
        }

        public User GetUserbyEmail(string email)
        {
            var users = GetCachedUsers();
            return users.FindRecord(x => string.Equals(x.Email,email,StringComparison.CurrentCultureIgnoreCase));
        }

        public string GetUserPassword(int userId)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.GetUserPassword(userId);
        }

        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(User userObject)
        {
            InsertOperationOutput<UserDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<UserDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int userId = -1;
            bool insertActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                var output = cloudServiceProxy.AddUserToApplication(new AddUserToApplicationInput
                    {
                        Email = userObject.Email,
                        Status = userObject.Status,
                        Description = userObject.Description
                    });
                if(output.OperationOutput != null && output.OperationOutput.Result == InsertOperationResult.Succeeded)
                {
                    insertActionSucc = true;
                    userObject = MapCloudUserToUser(output.OperationOutput.InsertedObject);
                }
                else
                {
                    insertActionSucc = false;
                }
            }
            else
            {
                string defPassword = "123456";
                string encryptedPassword = HashingUtility.ComputeHash(defPassword, "", null);

                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                insertActionSucc = dataManager.AddUser(userObject, encryptedPassword, out userId);

            }           

            if (insertActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                userObject.UserId = userId;
                insertOperationOutput.InsertedObject = UserDetailMapper(userObject);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUser(User userObject)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool updateActionSucc = dataManager.UpdateUser(userObject);
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(userObject);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(int userId, string password)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            bool updateActionSucc = dataManager.ResetPassword(userId, HashingUtility.ComputeHash(password, "", null));

            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }

            return updateOperationOutput;
        }


        public Vanrise.Entities.UpdateOperationOutput<UserProfile> EditUserProfile(UserProfile userProfileObject)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool updateActionSucc = dataManager.EditUserProfile(userProfileObject.Name, userProfileObject.UserId);
            UpdateOperationOutput<UserProfile> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserProfile>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = userProfileObject;
            }
            return updateOperationOutput;
        }

        public UserProfile LoadLoggedInUserProfile()
        {
            UserManager manager = new UserManager();
            return new UserProfile { UserId = SecurityContext.Current.GetLoggedInUserId(), Name = manager.GetUserbyId(SecurityContext.Current.GetLoggedInUserId()).Name };

        }

        public bool CheckUserName(string name)
        {
            var users = GetCachedUsers();
            return users.FindAllRecords(x => x.Name == name).Count() > 0;
        }

        public string GetUserName(int userId)
        {
            User user = GetUserbyId(userId);
            return user != null ? user.Name : null;
        }

        public string GetUsersNames(List<int> userIds)
        {
            if (userIds == null || userIds.Count == 0)
                return null;

            List<string> names = new List<string>();
            List<int> filteredUserIds = (from a in userIds
                                select a).Distinct().ToList();

            foreach (int userId in filteredUserIds)
            {
                User user = GetUserbyId(userId);
                if (user == null)
                    continue;
                names.Add(user.Name);
            }
            return string.Join<string>(",", names);
        }
        #endregion

        #region Private Methods

        private Dictionary<int, User> GetCachedUsers()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetUsers",
               () =>
               {                   
                   IEnumerable<User> users;
                   if(!TryGetUsersFromAuthServer(out users))                   
                   {
                       IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                       users = dataManager.GetUsers();
                   }
                   return users.ToDictionary(kvp => kvp.UserId, kvp => kvp);
               });
        }

        internal bool TryGetUsersFromAuthServer(out IEnumerable<User> users)
        {
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                GetApplicationUsersInput input = new GetApplicationUsersInput();
                var output = cloudServiceProxy.GetApplicationUsers(input);
                if (output != null && output.Users != null)
                    users = output.Users.Select(user => MapCloudUserToUser(user));
                else
                    users = null;
                return true;
            }
            else
            {
                users = null;
                return false;
            }
        }

        private User MapCloudUserToUser(CloudApplicationUser cloudApplicationUser)
        {
            return new User
            {
                UserId = cloudApplicationUser.User.UserId,
                Email = cloudApplicationUser.User.Email,
                Name = cloudApplicationUser.User.Name,
                LastLogin = cloudApplicationUser.User.LastLogin,
                Description = cloudApplicationUser.Description,
                Status = cloudApplicationUser.Status
            };
        }

        private ICloudService GetCloudServiceProxy()
        {
            var authServerManager = new CloudAuthServerManager();
            var authServer = authServerManager.GetAuthServer();
            if (authServer != null)
                return new CloudServiceProxy(authServer);
            else
                return null;
        }
        

        #endregion

        #region Private Classes

        public class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IUserDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            object _updateHandle;
            ICloudService _cloudServiceProxy = (new UserManager()).GetCloudServiceProxy();

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                if (_cloudServiceProxy != null)
                {
                    var output = _cloudServiceProxy.CheckApplicationUsersUpdated(new CheckApplicationUsersUpdatedInput { LastReceivedUpdateInfo = _updateHandle });
                    if (output != null)
                        _updateHandle = output.LastUpdateInfo;
                    return output.Updated;
                }
                else
                    return _dataManager.AreUsersUpdated(ref _updateHandle);
            }
        }


        #endregion

        #region Mappers

        private UserDetail UserDetailMapper(User userObject)
        {
            UserDetail userDetail = new UserDetail();
            userDetail.Entity = userObject;
            return userDetail;
        }


        private UserInfo UserInfoMapper(User userObject)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Name = userObject.Name;
            userInfo.Status = userObject.Status;
            userInfo.UserId = userObject.UserId;
            return userInfo;
        }

        #endregion
    }
}
