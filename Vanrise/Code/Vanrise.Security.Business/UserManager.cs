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

namespace Vanrise.Security.Business
{
    public class UserManager : IUserManager
    {
        static TimeSpan s_tempPasswordValidity;

        GroupManager _groupManager;
        SecurityManager _securityManager;
        ConfigManager _configManager;
        public UserManager()
        {

            _groupManager = new GroupManager();

            _securityManager = new SecurityManager();

            _configManager = new ConfigManager();

        }
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
            var userSettings = GetUserSetting(userId);
            if (userSettings == null)
                userSettings = new UserSetting();
            userSettings.LanguageId = languageId;
            bool updateActionSucc = dataManager.UpdateUserSetting(userSettings, userId);
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

            return users.MapRecords(UserInfoMapper, user => (filter == null || filter.GetOnlyTenantUsers || (filter.ExcludeInactive == false || IsUserEnable(user))));
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
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.GetUserPassword(userId);
        }

        public string GetUserTempPassword(int userId)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.GetUserTempPassword(userId);
        }

        public Vanrise.Entities.InsertOperationOutput<UserDetail> AddUser(UserToAdd userObject)
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
            User addedUser = null;
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
                    addedUser = GetUserbyId(output.OperationOutput.InsertedObject.User.UserId);
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

                string pwd;
                if (!String.IsNullOrWhiteSpace(userObject.Password))//password is not required when adding User, it is auto-generated
                {
                    string validationMessage;
                    if (!_securityManager.DoesPasswordMeetRequirement(userObject.Password, out validationMessage))
                    {
                        insertOperationOutput.Message = validationMessage;
                        return insertOperationOutput;
                    }

                    pwd = userObject.Password;
                }
                else
                {
                    if (!_configManager.ShouldSendEmailOnNewUser())
                    {
                        insertOperationOutput.Message = "Password cannot be empty.";
                        return insertOperationOutput;
                    }
                    PasswordGenerator passwordGenerator = new PasswordGenerator();
                    pwd = passwordGenerator.Generate();
                }
                string encryptedPassword = HashingUtility.ComputeHash(pwd, "", null);

                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                addedUser = new User
                {
                    Email = userObject.Email,
                    Name = userObject.Name,
                    EnabledTill = userObject.EnabledTill,
                    Description = userObject.Description,
                    TenantId = userObject.TenantId,
                    ExtendedSettings = userObject.ExtendedSettings,
                    Settings = new UserSetting()
                    {
                        PhotoFileId = userObject.PhotoFileId
                    }

                };
                insertActionSucc = dataManager.AddUser(addedUser, encryptedPassword, out userId);

                if (insertActionSucc)
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
            }

            if (insertActionSucc)
            {
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
            var cloudServiceProxy = GetCloudServiceProxy();
            User updatedUser = null;
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
                    updatedUser = GetUserbyId(output.OperationOutput.UpdatedObject.User.UserId);

                }
                else
                {
                    updateActionSucc = false;
                }
            }
            else
            {
                User currentUser = GetUserbyId(userObject.UserId);
                UserSetting setting = currentUser.Settings;
                 if (setting == null)
                      settings = new UserSetting();
                setting.PhotoFileId = userObject.PhotoFileId;               
                updatedUser = new User()
                {
                    UserId = userObject.UserId,
                    Email = userObject.Email,
                    Name = userObject.Name,
                    EnabledTill = userObject.EnabledTill,
                    Description = userObject.Description,
                    TenantId = userObject.TenantId,
                    ExtendedSettings = userObject.ExtendedSettings,
                    Settings = settings 
                };
                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.UpdateUser(updatedUser);
            }

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
        public bool DisableUser (int userId)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.DisableUser(userId);
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

                updateActionSucc = DisableUser(userObject.UserId);
            }

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var user = GetUserbyId(userObject.UserId);
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
        public bool EnableUser(int userId)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            return dataManager.EnableUser(userId);
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

                updateActionSucc = EnableUser(userObject.UserId);
            }

            if (updateActionSucc)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
                var user = GetUserbyId(userObject.UserId);
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

            bool updateActionSucc;
            var cloudServiceProxy = GetCloudServiceProxy();
            if (cloudServiceProxy != null)
            {
                throw new ArgumentNullException("cloudServiceProxy");
            }
            else
            {
                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.UnlockUser(userObject.UserId);
            }

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
        public Vanrise.Entities.UpdateOperationOutput<object> ResetPassword(int userId, string password)
        {
            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            string validationMessage;
            if (!_securityManager.DoesPasswordMeetRequirement(password, out validationMessage))
            {
                updateOperationOutput.Message = validationMessage;
                return updateOperationOutput;
            }

            User user = GetUserbyId(userId);

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

                string hashedPassword = HashingUtility.ComputeHash(password, "", null);
                IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
                updateActionSucc = dataManager.ResetPassword(userId, hashedPassword);


                if (updateActionSucc)
                {
                    new UserPasswordHistoryManager().AddPasswordHistory(userId, hashedPassword, true);
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
                                Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                                objects.Add("User", user);
                                objects.Add("Password", password);

                                Guid resetPasswordId = configManager.GetResetPasswordId();

                                VRMailManager vrMailManager = new VRMailManager();
                                vrMailManager.SendMail(resetPasswordId, objects);
                            }
                            catch (Exception ex)
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
                VRActionLogger.Current.LogObjectCustomAction(UserLoggableEntity.Instance, "Reset Password", true, user, null);
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

                PasswordGenerator pwdGenerator = new PasswordGenerator();
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
            UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();

            string validationMessage;
            if (!_securityManager.DoesPasswordMeetRequirement(password, out validationMessage))
            {
                updateOperationOutput.Message = validationMessage;
                updateOperationOutput.Result = UpdateOperationResult.Failed;
                return updateOperationOutput;
            }

            string loggedInUserTempPassword = GetUserTempPassword(user.UserId);
            if (!HashingUtility.VerifyHash(tempPassword, "", loggedInUserTempPassword))
                return new UpdateOperationOutput<object>() { Result = UpdateOperationResult.Failed };

            string hashedPass = HashingUtility.ComputeHash(password, "", null);

            bool updateActionSucc = dataManager.ActivatePassword(email, hashedPass, name);


            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            if (updateActionSucc)
            {
                new UserPasswordHistoryManager().AddPasswordHistory(user.UserId, hashedPass, false);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
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
            bool updateActionSucc = dataManager.EditUserProfile(userProfileObject.Name, userProfileObject.UserId, setting);
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
            long? photoFileId  = null;
            if(currentUser.Settings!=null)
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
            bool succ = dataManager.UpdateDisableTill(userId, disableTill);
            if (succ)
            {
                CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
            }
            return succ;
        }
        #endregion

        #region Private Methods

        private UserSetting  GetUserSetting (int userId)
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
    }
}