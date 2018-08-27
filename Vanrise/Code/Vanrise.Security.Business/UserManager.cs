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
using System.Configuration;
using RazorEngine.Templating;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.Security.Business
{
    public class UserManager : BaseBusinessEntityManager, IUserManager
    {
        GroupManager _groupManager;
        SecurityManager _securityManager;
        ConfigManager _configManager;

        public UserManager()
        {
            _groupManager = new GroupManager();
            _securityManager = new SecurityManager();
            _configManager = new ConfigManager();
        }

        #region Public Methods

        public IDataRetrievalResult<UserDetail> GetFilteredUsers(DataRetrievalInput<UserQuery> input)
        {
            var allItems = GetUsersByTenant();

            Func<User, bool> filterExpression = (itemObject) =>
                !itemObject.IsSystemUser
                &&
                 (input.Query.Name == null || itemObject.Name.ToLower().Contains(input.Query.Name.ToLower()))
                 &&
                 (input.Query.Email == null || itemObject.Email.ToLower().Contains(input.Query.Email.ToLower()));
            VRActionLogger.Current.LogGetFilteredAction(UserLoggableEntity.Instance, input);

            var resultProcessingHandler = new ResultProcessingHandler<UserDetail>()
            {
                ExportExcelHandler = new UserDetailExportExcelHandler()
            };

            return DataRetrievalManager.Instance.ProcessResult(input, allItems.ToBigResult(input, filterExpression, UserDetailMapper), resultProcessingHandler);
        }
        public UpdateOperationOutput<Guid?> UpdateLoggedInUserLanguage(Guid? languageId)
        {
            UpdateOperationOutput<Guid?> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<Guid?>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            var userId = SecurityContext.Current.GetLoggedInUserId();
            var lastModifiedBy = userId;
            var userSettings = GetUserSetting(userId);
            if (userSettings == null)
                userSettings = new UserSetting();
            userSettings.LanguageId = languageId;
            bool updateActionSucc = dataManager.UpdateUserSetting(userSettings, userId, lastModifiedBy);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = languageId;
            }
            return updateOperationOutput;
        }
        public Guid? GetLoggedInUserLanguageId()
        {
            var userId = SecurityContext.Current.GetLoggedInUserId();
            var userSettings = GetUserSetting(userId);
            return userSettings != null ? userSettings.LanguageId : null;
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

        public User GetUserHistoryDetailbyHistoryId(int userHistoryId)
        {
            VRObjectTrackingManager s_vrObjectTrackingManager = new VRObjectTrackingManager();
            var user = s_vrObjectTrackingManager.GetObjectDetailById(userHistoryId);
            return user.CastWithValidate<User>("User : historyId ", userHistoryId);
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
                    return users.MapRecords(UserInfoMapper, user => !excludedUserIds.Contains(user.UserId) || (filter.ExcludeInactive == false || IsUserEnable(user)));
                }
            }
            else
                users = GetUsersByTenant();

            Func<User, bool> filterExpression = (user) =>
            {
                if (user.IsSystemUser)
                {
                    if (filter == null || !filter.IncludeSystemUsers)
                        return false;
                }
                return (filter == null || filter.GetOnlyTenantUsers || (filter.ExcludeInactive == false || IsUserEnable(user)));
            };

            return users.MapRecords(UserInfoMapper, filterExpression);
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

        public User GetUserbyId(int userId, bool isViewedFromUI)
        {
            var users = GetCachedUsers();
            var user = users.GetRecord(userId);
            if (user != null && isViewedFromUI)
                VRActionLogger.Current.LogObjectViewed(UserLoggableEntity.Instance, user);
            return user;
        }

        public User GetUserbyId(int userId)
        {
            return GetUserbyId(userId, false);
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
            DateTime passwordChangeTime;
            return GetUserPassword(userId, out passwordChangeTime);
        }
        public string GetUserPassword(int userId, out DateTime passwordChangeTime)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.GetUserPassword(userId, out passwordChangeTime);
        }
        public string GetUserTempPassword(int userId)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.GetUserTempPassword(userId);
        }

        public InsertOperationOutput<UserDetail> AddRemoteUser(Guid connectionId, UserToAdd userToAdd)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Post<UserToAdd, InsertOperationOutput<UserDetail>>("/api/VR_Sec/Users/AddUser", userToAdd);
        }

        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(UserToAdd userObject)
        {
            InsertOperationOutput<UserDetail> insertOperationOutput = new Vanrise.Entities.InsertOperationOutput<UserDetail>();
            insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Failed;
            insertOperationOutput.InsertedObject = null;
            int userId = -1;
            bool insertActionSucc;


            string errorMessage;
            string encryptedPassword;
            string pwd;
            bool passwordCheckRequired;
            if (!ValidateUser(userObject.Email, userObject.SecurityProviderId, userObject.Password, out  encryptedPassword, out  pwd, out  passwordCheckRequired, out  errorMessage))
            {
                insertOperationOutput.Message = errorMessage;
                return insertOperationOutput;
            }

            User addedUser = null;

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            addedUser = new User
            {
                Email = userObject.Email,
                Name = userObject.Name,
                SecurityProviderId = userObject.SecurityProviderId,
                EnabledTill = userObject.EnabledTill,
                Description = userObject.Description,
                TenantId = userObject.TenantId,
                ExtendedSettings = userObject.ExtendedSettings,
                Settings = new UserSetting()
                {
                    PhotoFileId = userObject.PhotoFileId,
                    EnablePasswordExpiration = userObject.EnablePasswordExpiration
                },
                CreatedBy = loggedInUserId,
                LastModifiedBy = loggedInUserId

            };
            insertActionSucc = dataManager.AddUser(addedUser, encryptedPassword, out userId);
            if (insertActionSucc)
            {

                if (passwordCheckRequired)
                {
                    addedUser.UserId = userId;                    
                    Guid newUserId = _configManager.GetNewUserId();
                    if (_configManager.ShouldSendEmailOnNewUser())
                    {
                        Task sendMailTask = new Task(() =>
                        {
                            StartSendMailTask(newUserId, addedUser, pwd);
                        });
                        sendMailTask.Start();
                    }
                }

                new UserPasswordHistoryManager().AddPasswordHistory(userId, encryptedPassword, false);

                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();

                VRActionLogger.Current.TrackAndLogObjectAdded(UserLoggableEntity.Instance, addedUser);
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.Succeeded;
                if (userObject.GroupIds != null && userObject.GroupIds.Count() > 0)
                    foreach (int groupId in userObject.GroupIds)
                    {
                        UserGroup userGroup = new UserGroup()
                        {
                            UserId = addedUser.UserId,
                            GroupId = groupId
                        };
                        _groupManager.AssignUserToGroup(userGroup);
                    }
                insertOperationOutput.InsertedObject = UserDetailMapper(addedUser);
            }
            else
            {
                insertOperationOutput.Result = Vanrise.Entities.InsertOperationResult.SameExists;
            }

            return insertOperationOutput;
        }

        public bool ValidateUser(string email,Guid securityProviderId,string password, out string encryptedPassword, out string pwd,out bool passwordCheckRequired,  out string errorMessage)
        {
            errorMessage = null;

            encryptedPassword = null;
            pwd = null;
            passwordCheckRequired = false;

            if (!Utilities.IsEmailValid(email))
            {
                errorMessage = "Invalid Email Address.";
                return false;
            }

            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(securityProviderId);
            securityProvider.ThrowIfNull("securityProvider", securityProviderId);

            passwordCheckRequired = securityProvider.Settings.ExtendedSettings.PasswordCheckRequired;
            if (passwordCheckRequired)
            {
                if (!String.IsNullOrWhiteSpace(password))//password is not required when adding User, it is auto-generated
                {
                    string validationMessage;
                    if (!_securityManager.DoesPasswordMeetRequirement(password, out validationMessage))
                    {
                        errorMessage = validationMessage;
                        return false;
                    }

                    pwd = password;
                }
                else
                {
                    if (!_configManager.ShouldSendEmailOnNewUser())
                    {
                        errorMessage = "Password cannot be empty.";
                        return false;
                    }
                    PasswordGenerator passwordGenerator = new PasswordGenerator();
                    pwd = passwordGenerator.Generate();
                }
                encryptedPassword = HashingUtility.ComputeHash(pwd, "", null);
            }

            return true;
        }

        public Vanrise.Entities.UpdateOperationOutput<UserDetail> ChangeUserSecurityProvider(UserToChangeSecurityProvider userToChangeSecurityProvider)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            int userId = -1;
            bool updateActionSucc;

            string errorMessage;
            string encryptedPassword;
            string pwd;
            bool passwordCheckRequired;
            if (!ValidateUser(userToChangeSecurityProvider.Email, userToChangeSecurityProvider.SecurityProviderId, userToChangeSecurityProvider.Password, out  encryptedPassword, out  pwd,out  passwordCheckRequired, out  errorMessage))
            {
                updateOperationOutput.Message = errorMessage;
                return updateOperationOutput;
            }

            var currentUser = GetUserbyId(userToChangeSecurityProvider.UserId);
            UserSetting userSettings = null;
            if (currentUser.Settings != null)
            {
                userSettings = new UserSetting
                {
                    EnablePasswordExpiration = userToChangeSecurityProvider.EnablePasswordExpiration,
                    LanguageId = currentUser.Settings.LanguageId,
                    PhotoFileId = currentUser.Settings.PhotoFileId
                };
            }
            
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();

            updateActionSucc = dataManager.ChangeUserSecurityProvider(userToChangeSecurityProvider.UserId, userToChangeSecurityProvider.SecurityProviderId, userToChangeSecurityProvider.Email, encryptedPassword, userSettings, loggedInUserId);
            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var updatedUser = GetUserbyId(userToChangeSecurityProvider.UserId);
                if (passwordCheckRequired)
                {
                    new UserPasswordHistoryManager().AddPasswordHistory(userId, encryptedPassword, false);
                    Guid newUserId = _configManager.GetNewUserId();
                    if (_configManager.ShouldSendEmailOnNewUser())
                    {
                        Task sendMailTask = new Task(() =>
                        {
                            StartSendMailTask(newUserId, updatedUser, pwd);
                        });
                        sendMailTask.Start();
                    }
                }
                VRActionLogger.Current.TrackAndLogObjectAdded(UserLoggableEntity.Instance, updatedUser);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(updatedUser);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUserExpiration(int userId, DateTime? enabledTill)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            var user = GetUserbyId(userId);
            if (user.IsSystemUser)
                throw new Exception("Cannot update System User");
            user.EnabledTill = enabledTill;
            user.LastModifiedBy = SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSucc = dataManager.UpdateUser(user);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(user);
            }
            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UpdateUser(UserToUpdate userObject)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (!Utilities.IsEmailValid(userObject.Email))
            {
                updateOperationOutput.Message = "Invalid Email Address.";
                return updateOperationOutput;
            }

            bool updateActionSucc;

            User updatedUser = null;

            User currentUser = GetUserbyId(userObject.UserId);
            if (currentUser.IsSystemUser)
                throw new Exception("Cannot update System User");
            UserSetting settings = currentUser.Settings;
            if (settings == null)
                settings = new UserSetting();
            settings.PhotoFileId = userObject.PhotoFileId;
            settings.EnablePasswordExpiration = userObject.EnablePasswordExpiration;
            updatedUser = new User()
            {
                UserId = userObject.UserId,
                Email = userObject.Email,
                SecurityProviderId = userObject.SecurityProviderId,
                Name = userObject.Name,
                EnabledTill = userObject.EnabledTill,
                Description = userObject.Description,
                TenantId = userObject.TenantId,
                ExtendedSettings = userObject.ExtendedSettings,
                Settings = settings,
                LastModifiedBy = SecurityContext.Current.GetLoggedInUserId()
            };
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            updateActionSucc = dataManager.UpdateUser(updatedUser);


            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                VRActionLogger.Current.TrackAndLogObjectUpdated(UserLoggableEntity.Instance, updatedUser);

                var currentGroupsIds = _groupManager.GetUserGroups(userObject.UserId);
                var updatedGroupIds = userObject.GroupIds != null ? userObject.GroupIds : new List<int>();

                List<int> groupsToAdd = updatedGroupIds.FindAllRecords(x => !currentGroupsIds.Contains(x)).ToList();
                List<int> groupsToRemove = currentGroupsIds.FindAllRecords(y => !updatedGroupIds.Contains(y)).ToList();
                if (groupsToAdd != null && groupsToAdd.Count() > 0)
                    foreach (int groupId in groupsToAdd)
                    {
                        UserGroup userGroup = new UserGroup()
                        {
                            UserId = userObject.UserId,
                            GroupId = groupId
                        };
                        _groupManager.AssignUserToGroup(userGroup);
                    }
                if (groupsToRemove != null && groupsToRemove.Count() > 0)
                    foreach (int groupId in groupsToRemove)
                    {
                        UserGroup userGroup = new UserGroup()
                        {
                            UserId = userObject.UserId,
                            GroupId = groupId
                        };
                        _groupManager.UnAssignUserToGroup(userGroup);
                    }

                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(updatedUser);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.SameExists;
            }

            return updateOperationOutput;
        }
        public Vanrise.Entities.UpdateOperationOutput<UserDetail> DisableUser(int userId)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            int lastModifiedBy = SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSucc = dataManager.DisableUser(userId, lastModifiedBy);

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var user = GetUserbyId(userId);
                VRActionLogger.Current.LogObjectCustomAction(UserLoggableEntity.Instance, "Disable", true, user, null);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(user);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
        }

        public bool IsUserDisabledTill(User user, out TimeSpan? disableTill)
        {
            disableTill = null;
            if (user.DisabledTill.HasValue && user.DisabledTill.Value > DateTime.Now)
            {
                disableTill = user.DisabledTill.Value - DateTime.Now;
                return true;
            }
            return false;
        }
        public bool IsUserEnable(User user)
        {
            if (user.EnabledTill.HasValue && user.EnabledTill.Value < DateTime.Now)
                return false;
            if (user.DisabledTill.HasValue && user.DisabledTill.Value > DateTime.Now)
                return false;
            return true;
        }
        public bool IsUserEnable(int userId, out  UserStatus userStatus)
        {
            var user = GetUserbyId(userId);
            user.ThrowIfNull("user", userId);
            return IsUserEnable(user, out userStatus);
        }
        public bool IsUserEnable(User user, out UserStatus userStatus)
        {
            userStatus = UserStatus.Active;
            if (user.EnabledTill.HasValue && user.EnabledTill.Value < DateTime.Now)
            {
                userStatus = UserStatus.Inactive;
                return false;
            }
            if (user.DisabledTill.HasValue && user.DisabledTill.Value > DateTime.Now)
            {
                userStatus = UserStatus.Locked;
                return false;
            }
            return true;
        }

        public Vanrise.Entities.UpdateOperationOutput<UserDetail> EnableUser(int userId)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            int lastModifiedBy = SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSucc = dataManager.EnableUser(userId, lastModifiedBy);

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var user = GetUserbyId(userId);
                VRActionLogger.Current.LogObjectCustomAction(UserLoggableEntity.Instance, "Enable", true, user, null);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(user);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<UserDetail> UnlockUser(User userObject)
        {
            UpdateOperationOutput<UserDetail> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<UserDetail>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            int lastModifiedBy = SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSucc = dataManager.UnlockUser(userObject.UserId, lastModifiedBy);

            if (updateActionSucc)
            {
                UserFailedLoginManager userFailedLoginManager = new UserFailedLoginManager();
                userFailedLoginManager.DeleteUserFailedLogin(userObject.UserId);
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var user = GetUserbyId(userObject.UserId);
                VRActionLogger.Current.LogObjectCustomAction(UserLoggableEntity.Instance, "Unlock", true, user, null);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
                updateOperationOutput.UpdatedObject = UserDetailMapper(user);
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(string email, string password)
        {
            User user = GetUserbyEmail(email);
            if (user == null)
            {
                UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
                updateOperationOutput.Message = "Invalid Email";
            }
            return ResetPassword(user.UserId, password);
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(int userId, string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            User user = GetUserbyId(userId);
            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(user.SecurityProviderId);
            securityProvider.ThrowIfNull("securityProvider", user.SecurityProviderId);

            SecurityProviderResetPasswordContext resetPasswordContext = new SecurityProviderResetPasswordContext() { User = user, Password = password };
            bool updateActionSucc = securityProvider.Settings.ExtendedSettings.ResetPassword(resetPasswordContext);

            if (updateActionSucc)
            {
                VRActionLogger.Current.LogObjectCustomAction(UserLoggableEntity.Instance, "Reset Password", true, user, null);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
                updateOperationOutput.Message = resetPasswordContext.ValidationMessage;
                updateOperationOutput.ShowExactMessage = resetPasswordContext.ShowExactMessage;
            }

            return updateOperationOutput;
        }

        public UpdateOperationOutput<object> ForgotPassword(string email)
        {
            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            var user = new UserManager().GetUserbyEmail(email);
            if (user == null)
                return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(user.SecurityProviderId);
            securityProvider.ThrowIfNull("securityProvider", user.SecurityProviderId);

            SecurityProviderForgotPasswordContext resetPasswordContext = new SecurityProviderForgotPasswordContext() { User = user };
            bool updateActionSucc = securityProvider.Settings.ExtendedSettings.ForgotPassword(resetPasswordContext);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.Message = resetPasswordContext.ValidationMessage;
                updateOperationOutput.ShowExactMessage = resetPasswordContext.ShowExactMessage;
            }

            return updateOperationOutput;
        }

        public UpdateOperationOutput<object> ActivatePassword(string email, string password, string tempPassword)
        {
            var user = new UserManager().GetUserbyEmail(email);
            if (user == null)
                return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(user.SecurityProviderId);
            securityProvider.ThrowIfNull("securityProvider", user.SecurityProviderId);

            SecurityProviderActivatePasswordContext context = new SecurityProviderActivatePasswordContext() { User = user, Password = password, TempPassword = tempPassword };
            bool updateActionSucc = securityProvider.Settings.ExtendedSettings.ActivatePassword(context);
            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.Message = context.ValidationMessage;
                updateOperationOutput.ShowExactMessage = context.ShowExactMessage;
            }

            return updateOperationOutput;
        }

        public Vanrise.Entities.UpdateOperationOutput<UserProfile> EditUserProfile(UserProfile userProfileObject)
        {
            User currentUser = GetUserbyId(userProfileObject.UserId);
            UserSetting setting = currentUser.Settings;
            if (setting == null)
                setting = new UserSetting();
            setting.PhotoFileId = userProfileObject.PhotoFileId;

            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            int lastModifiedBy = SecurityContext.Current.GetLoggedInUserId();
            bool updateActionSucc = dataManager.EditUserProfile(userProfileObject.Name, userProfileObject.UserId, setting, lastModifiedBy);
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
            User currentUser = manager.GetUserbyId(SecurityContext.Current.GetLoggedInUserId());
            long? photoFileId = null;
            if (currentUser.Settings != null)
                photoFileId = currentUser.Settings.PhotoFileId;
            return new UserProfile { UserId = SecurityContext.Current.GetLoggedInUserId(), Name = currentUser.Name, PhotoFileId = photoFileId };

        }

        public bool CheckUserName(string name)
        {
            var users = GetCachedUsers();
            return users.FindAllRecords(x => x.Name == name).Count() > 0;
        }

        public string GetUserName(int userId)
        {
            User user = GetUserbyId(userId);
            return GetUserName(user);
        }

        public string GetUserName(User user)
        {
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

        public UserDTO GetUserDetailsByEmail(string email)
        {
            User user = GetUserbyEmail(email);
            return GetUserDetails(user);
        }

        public UserDTO GetUserDetailsById(int userId)
        {
            User user = GetUserbyId(userId);
            return GetUserDetails(user);
        }

        public bool UpdateDisableTill(int userId, DateTime disableTill)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            int lastModifiedBy = userId;
            bool succ = dataManager.UpdateDisableTill(userId, disableTill, lastModifiedBy);
            if (succ)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return succ;
        }

        public int? GetSystemUserId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSystemUserId",
                () =>
                {
                    User systemUser = null;
                    var allUsers = GetCachedUsers();

                    if (allUsers != null)
                        systemUser = allUsers.Values.FindRecord(user => user.IsSystemUser);
                    return systemUser != null ? systemUser.UserId : default(int?);
                });
        }

        public IEnumerable<EmailInfo> GetRemoteEmailInfo(Guid connectionId, string serializedFilter)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            return connectionSettings.Get<IEnumerable<EmailInfo>>(string.Format("/api/VR_Sec/Users/GetEmailInfo?serializedFilter={0}", serializedFilter));
        }

        public IEnumerable<EmailInfo> GetEmailInfo(EmailInfoFilter filter)
        {
            var users = GetCachedUsers();
            if (users == null)
                return null;

            Func<User, bool> filterExpression = (user) =>
            {
                return true;
            };

            return users.MapRecords(EmailInfoMapper, filterExpression);
        }

        private EmailInfo EmailInfoMapper(User user)
        {
            return new EmailInfo()
            {
                Email = user.Email,
                Name = user.Name,
                Description = user.Description
            };
        }

        #endregion

        #region Private Methods

        private UserSetting GetUserSetting(int userId)
        {
            var user = GetUserbyId(userId);
            user.ThrowIfNull("user is null");
            return user.Settings;

        }
        private UserDTO GetUserDetails(User user)
        {
            if (user == null)
                return null;
            GroupManager groupManager = new GroupManager();
            List<int> assignedGroupIds = groupManager.GetAssignedUserGroups(user.UserId);

            return new UserDTO()
            {
                Entity = user,
                AssignedGroupIds = assignedGroupIds
            };
        }

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
                   IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                   IEnumerable<User> users = dataManager.GetUsers();
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

        private static void StartSendMailTask(Guid newUserID, object user, string pwd)
        {
            Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
            objects.Add("User", user);
            objects.Add("Password", pwd);
            VRMailManager vrMailManager = new VRMailManager();
            try
            {
                vrMailManager.SendMail(newUserID, objects);
            }
            catch (Exception ex)
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
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreUsersUpdated(ref _updateHandle);
            }
        }

        public class UserLoggableEntity : VRLoggableEntityBase
        {
            public static UserLoggableEntity Instance = new UserLoggableEntity();

            private UserLoggableEntity()
            {

            }

            static UserManager s_userManager = new UserManager();

            public override string EntityUniqueName
            {
                get { return "VR_Security_User"; }
            }

            public override string ModuleName
            {
                get { return "Security"; }
            }

            public override string EntityDisplayName
            {
                get { return "User"; }
            }

            public override string ViewHistoryItemClientActionName
            {
                get { return "VR_Security_User_ViewHistoryItem"; }
            }

            public override object GetObjectId(IVRLoggableEntityGetObjectIdContext context)
            {
                User user = context.Object.CastWithValidate<User>("context.Object");
                return user.UserId;
            }

            public override string GetObjectName(IVRLoggableEntityGetObjectNameContext context)
            {
                User user = context.Object.CastWithValidate<User>("context.Object");
                return s_userManager.GetUserName(user);
            }
        }

        private class UserDetailExportExcelHandler : ExcelExportHandler<UserDetail>
        {
            public override void ConvertResultToExcelData(IConvertResultToExcelDataContext<UserDetail> context)
            {
                var sheet = new ExportExcelSheet()
                {
                    SheetName = "Users",
                    Header = new ExportExcelHeader() { Cells = new List<ExportExcelHeaderCell>() }
                };

                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "ID" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Name" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Email" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Status" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Enable Till", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.Date });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Last Login Date", CellType = ExcelCellType.DateTime, DateTimeType = DateTimeType.DateTime });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Groups" });
                sheet.Header.Cells.Add(new ExportExcelHeaderCell() { Title = "Description" });

                sheet.Rows = new List<ExportExcelRow>();
                if (context.BigResult != null && context.BigResult.Data != null)
                {
                    foreach (var record in context.BigResult.Data)
                    {
                        if (record.Entity != null)
                        {
                            var row = new ExportExcelRow() { Cells = new List<ExportExcelCell>() };
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.UserId });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Name });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Email });
                            row.Cells.Add(new ExportExcelCell() { Value = Vanrise.Common.Utilities.GetEnumDescription(record.Status) });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.EnabledTill });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.LastLogin });
                            row.Cells.Add(new ExportExcelCell() { Value = record.GroupNames });
                            row.Cells.Add(new ExportExcelCell() { Value = record.Entity.Description });
                            sheet.Rows.Add(row);
                        }
                    }
                }

                context.MainSheet = sheet;
            }
        }



        #endregion

        #region Mappers

        private UserDetail UserDetailMapper(User userObject)
        {
            UserStatus status = UserStatus.Active;
            bool isActive = IsUserEnable(userObject, out status);
            UserDetail userDetail = new UserDetail();
            userDetail.Entity = userObject;
            userDetail.Status = status;
            userDetail.GroupNames = GetGroupNames(userObject.UserId);

            var securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(userObject.SecurityProviderId);
            securityProvider.ThrowIfNull("securityProvider", userObject.SecurityProviderId);

            userDetail.SupportPasswordManagement = securityProvider.Settings.ExtendedSettings.SupportPasswordManagement;
            return userDetail;
        }

        private UserInfo UserInfoMapper(User userObject)
        {
            UserStatus status = UserStatus.Active;
            bool isActive = IsUserEnable(userObject, out status);
            UserInfo userInfo = new UserInfo();
            userInfo.Name = userObject.Name;
            userInfo.Status = status;
            userInfo.UserId = userObject.UserId;
            return userInfo;
        }

        private string GetGroupNames(int userId)
        {
            var groupIds = _groupManager.GetUserGroups(userId);
            List<string> names = new List<string>();
            foreach (var id in groupIds)
            {
                string groupName = _groupManager.GetGroupName(id);
                names.Add(groupName);
            }
            return string.Join(",", names);
        }

        #endregion

        #region IBusinessEntityManager
        public override List<dynamic> GetAllEntities(IBusinessEntityGetAllContext context)
        {
            return GetUsers().Select(itm => itm as dynamic).ToList();
        }

        public override dynamic GetEntity(IBusinessEntityGetByIdContext context)
        {
            return GetUserbyId(context.EntityId);
        }

        public override dynamic GetEntityId(IBusinessEntityIdContext context)
        {
            var user = context.Entity as User;
            return user.UserId;
        }

        public override string GetEntityDescription(IBusinessEntityDescriptionContext context)
        {
            return GetUserName(Convert.ToInt32(context.EntityId));
        }

        public override dynamic MapEntityToInfo(IBusinessEntityMapToInfoContext context)
        {
            throw new NotImplementedException();
        }

        public override bool IsCacheExpired(IBusinessEntityIsCacheExpiredContext context, ref DateTime? lastCheckTime)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().IsCacheExpired(ref lastCheckTime);
        }

        public override dynamic GetParentEntityId(IBusinessEntityGetParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<dynamic> GetIdsByParentEntityId(IBusinessEntityGetIdsByParentEntityIdContext context)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}