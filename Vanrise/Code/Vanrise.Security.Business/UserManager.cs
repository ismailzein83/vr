﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;
using Vanrise.Caching;
using Vanrise.Common;
using System.Configuration;
using RazorEngine.Templating;
using Vanrise.Common.Business;

namespace Vanrise.Security.Business
{
    public class UserManager : IUserManager
    {
        static TimeSpan s_tempPasswordValidity;

        static UserManager()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["TempPasswordValidity"], out s_tempPasswordValidity))
                s_tempPasswordValidity = new TimeSpan(1, 0, 0);
        }

        #region Public Methods

        public IDataRetrievalResult<UserDetail> GetFilteredUsers(DataRetrievalInput<UserQuery> input)
        {
            var allItems = GetUsersByTenant();

            Func<User, bool> filterExpression = (itemObject) =>
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.Email == null || itemObject.Email.ToLower().Contains(input.Query.Email.ToLower()));

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, UserDetailMapper));
        }

        public T GetUserExtendedSettings<T>(int userId) where T : class
        {
            User user = GetUserbyId(userId);
            string extendedSettingName = typeof(T).FullName;

            Object exitingExtendedSettings;
            if (user.ExtendedSettings != null && user.ExtendedSettings.TryGetValue(extendedSettingName, out exitingExtendedSettings))
                return exitingExtendedSettings as T;

            return null;
        }

        public IEnumerable<User> GetUsers()
        {
            var users = GetUsersByTenant();
            return users.Values;
        }

        public IEnumerable<UserInfo> GetUsersInfo(UserFilter filter)
        {
            Dictionary<int, User> users;

            if (filter != null)
            {
                users = GetUsersByTenant(!filter.GetOnlyTenantUsers, filter.TenantId);

                if (filter.Filters != null && filter.Filters.Count > 0 && users != null && users.Count > 0)
                {
                    users = FilterUsers(users, filter.Filters);
                }

                if (filter.EntityType != null && filter.EntityId != null)
                {
                    PermissionManager permissionManager = new PermissionManager();
                    IEnumerable<Permission> entityPermissions = permissionManager.GetEntityPermissions((EntityType)filter.EntityType, filter.EntityId);

                    IEnumerable<int> excludedUserIds = entityPermissions.MapRecords(permission => Convert.ToInt32(permission.HolderId), permission => permission.HolderType == HolderType.USER);
                    return users.MapRecords(UserInfoMapper, user => !excludedUserIds.Contains(user.UserId) || (filter.ExcludeInactive == true && IsUserEnable(user)));
                }
            }
            else
                users = GetUsersByTenant();

            return users.MapRecords(UserInfoMapper, user => (filter == null || filter.GetOnlyTenantUsers || (filter.ExcludeInactive == true && IsUserEnable(user))));
        }

        private Dictionary<int, User> FilterUsers(Dictionary<int, User> users, List<IUserFilter> filters)
        {
            Dictionary<int, User> validUsers = new Dictionary<int, User>();
            foreach (KeyValuePair<int, User> user in users)
            {
                if (filters.Where(itm => itm.IsExcluded(user.Value)).Count() == 0)
                {
                    validUsers.Add(user.Key, user.Value);
                }
            }
            return validUsers.Count > 0 ? validUsers : null;
        }

        public User GetUserbyId(int userId)
        {
            var users = GetCachedUsers();
            return users.GetRecord(userId);
        }

        public User GetUserbyEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;

            var users = GetCachedUsersByEmail();
            return users.GetRecord(email.ToLower());
        }

        public string GetUserPassword(int userId)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.GetUserPassword(userId);
        }

        public string GetUserTempPassword(int userId)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.GetUserTempPassword(userId);
        }

        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(User userObject)
        {
            InsertOperationOutput<UserDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<UserDetail>();

            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int userId = -1;
            bool insertActionSucc;

            if (!Utilities.IsEmailValid(userObject.Email))
            {
                insertOperationOutput.Message = "Invalid Email Address.";
                return insertOperationOutput;
            }

            var cloudServiceProxy = GetCloudServiceProxy();

            if (cloudServiceProxy != null)
            {
                var output = cloudServiceProxy.AddUserToApplication(new AddUserToApplicationInput
                    {
                        Email = userObject.Email,
                        EnabledTill = userObject.EnabledTill,
                        Description = userObject.Description,
                        TenantId = userObject.TenantId
                    });
                if (output.OperationOutput != null && output.OperationOutput.Result == InsertOperationResult.Succeeded)
                {
                    insertActionSucc = true;
                    userObject = MapCloudUserToUser(output.OperationOutput.InsertedObject);
                }
                else
                {
                    insertActionSucc = false;
                    if (output.OperationOutput != null && output.OperationOutput.Result == InsertOperationResult.Failed)
                    {
                        return new InsertOperationOutput<UserDetail>()
                        {
                            Message = output.OperationOutput.Message,
                            Result = output.OperationOutput.Result,
                            ShowExactMessage = output.OperationOutput.ShowExactMessage
                        };
                    }
                }
            }
            else
            {
                PasswordGenerator passwordGenerator = new PasswordGenerator();
                string pwd = passwordGenerator.Generate();
                string encryptedPassword = HashingUtility.ComputeHash(pwd, "", null);

                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                insertActionSucc = dataManager.AddUser(userObject, encryptedPassword, out userId);
                if (insertActionSucc)
                {
                    //EmailTemplateManager emailTemplateManager = new EmailTemplateManager();
                    //EmailTemplate template = emailTemplateManager.GeEmailTemplateByType(Constants.NewPasswordType);
                    //PasswordEmailContext context = new PasswordEmailContext() { Name = userObject.Name, Password = pwd };
                    //emailTemplateManager.SendEmail(userObject.Email, template.GetParsedBodyTemplate(context), template.GetParsedSubjectTemplate(context));

                   
                    ConfigManager cManager = new ConfigManager();
                    Guid newUserId = cManager.GetNewUserId();
                    if (cManager.ShouldSendEmailOnNewUser())
                    {
                        Task sendMailTask = new Task(() =>
                        {
                            StartSendMailTask(newUserId, userObject, pwd);
                        });
                        sendMailTask.Start();
                    }
                   
                }
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
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                var output = cloudServiceProxy.UpdateUserToApplication(new UpdateUserToApplicationInput
                {
                    UserId = userObject.UserId,
                    EnabledTill = userObject.EnabledTill,
                    Description = userObject.Description,
                    //TenantId = userObject.TenantId
                });
                if (output.OperationOutput != null && output.OperationOutput.Result == UpdateOperationResult.Succeeded)
                {
                    updateActionSucc = true;
                }
                else
                {
                    updateActionSucc = false;
                }
            }
            else
            {
                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.UpdateUser(userObject);
            }

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

        public Vanrise.Entities.UpdateOperationOutput<UserDetail> DisableUser(User userObject)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                throw new NullReferenceException("cloudServiceProxy");
            }
            else
            {
                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.DisableUser(userObject.UserId);
            }

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                var user = GetUserbyId(userObject.UserId);
                updateOperationOutput.UpdatedObject = UserDetailMapper(user);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
        }

        public bool IsUserEnable(User user)
        {
            return (!user.EnabledTill.HasValue || user.EnabledTill.Value > DateTime.Now);
        }
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> EnableUser(User userObject)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                throw new ArgumentNullException("cloudServiceProxy");
            }
            else
            {
                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.EnableUser(userObject.UserId);
            }

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                var user = GetUserbyId(userObject.UserId);
                updateOperationOutput.UpdatedObject = UserDetailMapper(user);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(int userId, string password)
        {
            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                var output = cloudServiceProxy.ResetUserPasswordApplication(new ResetUserPasswordApplicationInput
                {
                    UserId = userId
                });
                if (output.OperationOutput != null && output.OperationOutput.Result == UpdateOperationResult.Succeeded)
                {
                    updateActionSucc = true;
                }
                else
                {
                    updateActionSucc = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(password))
                    throw new ArgumentNullException("password");

                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.ResetPassword(userId, HashingUtility.ComputeHash(password, "", null));
                if (updateActionSucc)
                {
                    //EmailTemplateManager emailTemplateManager = new EmailTemplateManager();
                    //EmailTemplate template = emailTemplateManager.GeEmailTemplateByType(Constants.ForgotPasswordType);
                    //PasswordEmailContext context = new PasswordEmailContext() { Name = user.Name, Password = pwd };
                    //emailTemplateManager.SendEmail(email, template.GetParsedBodyTemplate(context), template.GetParsedSubjectTemplate(context));
                    var configManager = new ConfigManager();
                    if (configManager.ShouldSendEmailOnResetPasswordByAdmin())
                    {
                        Task taskSendMail = new Task(() =>
                        {
                            try
                            {
                                User user = GetUserbyId(userId);

                                Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                                objects.Add("User", user);
                                objects.Add("Password", password);

                                Guid resetPasswordId = configManager.GetResetPasswordId();

                                VRMailManager vrMailManager = new VRMailManager();
                                vrMailManager.SendMail(resetPasswordId, objects);
                            }
                            catch(Exception ex)
                            {
                                LoggerFactory.GetExceptionLogger().WriteException(ex);
                            }
                        });
                        taskSendMail.Start();
                    }
                }
            }

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
        }

        public UpdateOperationOutput<object> ForgotPassword(string email)
        {
            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            bool updateActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                var output = cloudServiceProxy.ForgotUserPasswordApplication(new ForgotUserPasswordApplicationInput
                {
                    Email = email
                });
                if (output.OperationOutput != null && output.OperationOutput.Result == UpdateOperationResult.Succeeded)
                {
                    updateActionSucc = true;
                }
                else
                {
                    updateActionSucc = false;
                }
            }
            else
            {
                UserManager userManager = new UserManager();
                var user = userManager.GetUserbyEmail(email);

                if (user == null)
                    return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

                Vanrise.Common.PasswordGenerator pwdGenerator = new PasswordGenerator();
                string pwd = pwdGenerator.Generate();

                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.UpdateTempPasswordByEmail(email, HashingUtility.ComputeHash(pwd, "", null), DateTime.Now.Add(s_tempPasswordValidity));
                if (updateActionSucc)
                {
                    //EmailTemplateManager emailTemplateManager = new EmailTemplateManager();
                    //EmailTemplate template = emailTemplateManager.GeEmailTemplateByType(Constants.ForgotPasswordType);
                    //PasswordEmailContext context = new PasswordEmailContext() { Name = user.Name, Password = pwd };
                    //emailTemplateManager.SendEmail(email, template.GetParsedBodyTemplate(context), template.GetParsedSubjectTemplate(context));

                    Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                    objects.Add("User", user);
                    objects.Add("Password", pwd);

                    Guid forgotPasswordId = new ConfigManager().GetForgotPasswordId();

                    VRMailManager vrMailManager = new VRMailManager();
                    vrMailManager.SendMail(forgotPasswordId, objects);
                }
            }

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
        }

        public UpdateOperationOutput<object> UpdateTempPasswordById(int userId, string password, DateTime? passwordValidTill)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            bool updateActionSucc = dataManager.UpdateTempPasswordById(userId, HashingUtility.ComputeHash(password, "", null), passwordValidTill);

            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }

            return updateOperationOutput;
        }

        public UpdateOperationOutput<object> UpdateTempPasswordByEmail(string email, string password, DateTime? passwordValidTill)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            bool updateActionSucc = dataManager.UpdateTempPasswordByEmail(email, HashingUtility.ComputeHash(password, "", null), passwordValidTill);

            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }

            return updateOperationOutput;
        }

        public UpdateOperationOutput<object> ActivatePassword(string email, string password, string name, string tempPassword)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            UserManager userManager = new UserManager();
            var user = userManager.GetUserbyEmail(email);
            if (user == null)
                return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

            string loggedInUserTempPassword = GetUserTempPassword(user.UserId);
            if (!HashingUtility.VerifyHash(tempPassword, "", loggedInUserTempPassword))
                return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

            bool updateActionSucc = dataManager.ActivatePassword(email, HashingUtility.ComputeHash(password, "", null), name);

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


        private Dictionary<int, User> GetUsersByTenant(bool getRelatedTenantsUser = true, int? selectedTenantId = null)
        {
            UserManager userManager = new UserManager();
            int? tenantId = null;
            int? userId;
            if (selectedTenantId.HasValue)
            {
                tenantId = selectedTenantId;
            }
            else if (SecurityContext.Current.TryGetLoggedInUserId(out userId))
            {
                User user = userManager.GetUserbyId(userId.Value);
                tenantId = user.TenantId;
            }

            TenantManager tenantManager = new TenantManager();
            List<int> relatedTenants = tenantManager.GetTenantsByTenantId(tenantId, getRelatedTenantsUser).Select(itm => itm.TenantId).ToList();

            var cachedUsers = GetCachedUsers();

            Dictionary<int, User> result = new Dictionary<int, User>();
            foreach (KeyValuePair<int, User> userItem in cachedUsers)
            {
                if (relatedTenants.Contains(userItem.Value.TenantId))
                    result.Add(userItem.Key, userItem.Value);
            }
            return result;
        }
        private Dictionary<int, User> GetCachedUsers()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetUsers",
               () =>
               {
                   IEnumerable<User> users;
                   if (!TryGetUsersFromAuthServer(out users))
                   {
                       IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                       users = dataManager.GetUsers();
                   }
                   return users.ToDictionary(kvp => kvp.UserId, kvp => kvp);
               });
        }

        private Dictionary<string, User> GetCachedUsersByEmail()
        {
            return CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCachedUsersByEmail",
               () =>
               {
                   var users = GetCachedUsers();
                   return users.ToDictionary(itm => itm.Value.Email.ToLower(), itm => itm.Value);
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
                EnabledTill = cloudApplicationUser.EnabledTill,
                TenantId = cloudApplicationUser.User.TenantId
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
        private static void StartSendMailTask(Guid newUserID,object user , string pwd)
        {
            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("User", user);
            objects.Add("Password", pwd);
            VRMailManager vrMailManager = new VRMailManager();
            try
            {               
                vrMailManager.SendMail(newUserID, objects);
            }
            catch(Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
            }
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
            userDetail.Status = IsUserEnable(userObject) ? UserStatus.Active : UserStatus.Inactive;
            return userDetail;
        }


        private UserInfo UserInfoMapper(User userObject)
        {
            UserInfo userInfo = new UserInfo();
            userInfo.Name = userObject.Name;
            userInfo.Status = IsUserEnable(userObject) ? UserStatus.Active : UserStatus.Inactive;
            userInfo.UserId = userObject.UserId;
            return userInfo;
        }

        #endregion
    }

    public class UserVRActionObjectNameResolver : IVRActionObjectNameResolver
    {
        static UserManager s_userManager = new UserManager();
        public string GetObjectName(IVRActionObjectNameResolverContext context)
        {
            int userId;
            if (!int.TryParse(context.ObjectId, out userId))
                throw new Exception(String.Format("Cannot parse context.ObjectId '{0}' to int", context.ObjectId));
            return s_userManager.GetUserName(userId);
        }
    }

}
