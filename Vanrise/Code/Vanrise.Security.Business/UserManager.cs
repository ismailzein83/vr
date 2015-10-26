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

        private Dictionary<int, User> GetCachedUsers()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetUsers",
               () =>
               {
                   IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                   IEnumerable<User> users = dataManager.GetUsers();
                   return users.ToDictionary(kvp => kvp.UserId, kvp => kvp);
               });
        }

        public IDataRetrievalResult<UserDetail> GetFilteredUsers(DataRetrievalInput<UserQuery> input)
        {
            var allItems = GetCachedUsers();

            Func<User, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.Email == null || itemObject.Email.ToLower().Contains(input.Query.Email.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, UserDetailMapper));
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IUserDataManager _dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreUsersUpdated(ref _updateHandle);
            }
        }


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

        public IEnumerable<UserInfo> GetUsers()
        {
            var users = GetCachedUsers();
            return users.MapRecords(UserInfoMapper);
        }

        public User GetUserbyId(int userId)
        {
            var users = GetCachedUsers();
            return users.GetRecord(userId);
        }


        public User GetUserbyEmail(string email)
        {
            var users = GetCachedUsers();
            return users.FindRecord(x=>x.Email ==  email);
        }

        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(User userObject)
        {
            InsertOperationOutput<UserDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<UserDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int userId = -1;

            string defPassword = "123456";
            userObject.Password = HashingUtility.ComputeHash(defPassword, "", null);

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            bool insertActionSucc = dataManager.AddUser(userObject, out userId);

            if (insertActionSucc)
            {
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
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(userObject);
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


        public List<User> GetMembers(int groupId)
        {
            IUserDataManager datamanager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return datamanager.GetMembers(groupId);
        }

      

       
    }
}
