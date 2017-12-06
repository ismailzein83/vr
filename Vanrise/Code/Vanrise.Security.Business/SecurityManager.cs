using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityManager
    {
        static TimeSpan s_tokenExpirationInterval;

        static SecurityManager()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Security_TokenExpirationInterval"], out s_tokenExpirationInterval))
                s_tokenExpirationInterval = TimeSpan.FromDays(1);
        }

        #region Public Methods

        public AuthenticateOperationOutput<AuthenticationToken> Authenticate(string email, string password)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();

            UserManager manager = new UserManager();
            User user = manager.GetUserbyEmail(email);

            AuthenticateOperationOutput<AuthenticationToken> authenticationOperationOutput = new AuthenticateOperationOutput<AuthenticationToken>();
            authenticationOperationOutput.Result = AuthenticateOperationResult.Failed;
            authenticationOperationOutput.AuthenticationObject = null;

            if (user != null)
            {
                authenticationOperationOutput.LoggedInUser = user;

                AuthenticationToken authToken = new AuthenticationToken();
                authToken.TokenName = SecurityContext.SECURITY_TOKEN_NAME;
                authToken.Token = null;
                authToken.UserName = user.Email;
                authToken.UserDisplayName = user.Name;
                authToken.ExpirationIntervalInMinutes = (int)Math.Ceiling(s_tokenExpirationInterval.TotalMinutes);

                ConfigManager cManager = new ConfigManager();

                TimeSpan? disableTillTime;
                if (manager.IsUserDisabledTill(user, out disableTillTime))
                {
                    authenticationOperationOutput.Result = AuthenticateOperationResult.Inactive;
                    authenticationOperationOutput.Message = string.Format("User is locked, try after {0} minutes, {1} seconds", disableTillTime.Value.Minutes, disableTillTime.Value.Seconds);
                    return authenticationOperationOutput;
                }

                if (cManager.GetFailedInterval().HasValue)
                {
                    UserFailedLoginManager userFailedLoginManager = new UserFailedLoginManager();
                    var failedLogins = userFailedLoginManager.GetUserFailedLoginByUserId(user.UserId, DateTime.Now.AddMinutes(-1), DateTime.Now.AddTicks(cManager.GetFailedInterval().Value.Ticks));
                    if (failedLogins.Count() == cManager.GetMaxFailedTries())
                    {
                        UserManager userManager = new UserManager();
                        DateTime disableTill = DateTime.Now.AddMinutes(cManager.GetLockForMinutes());
                        var disableTillTimeSpan = disableTill - DateTime.Now;

                        SendNotificationMail(user, cManager);

                        if (userManager.UpdateDisableTill(user.UserId, disableTill))
                        {
                            VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "User is locked after multiple failed logins");
                            authenticationOperationOutput.Result = AuthenticateOperationResult.Inactive;
                            authenticationOperationOutput.Message = string.Format("User is locked, try after {0} minutes, {1} seconds", disableTillTimeSpan.Minutes, disableTillTimeSpan.Seconds);
                            return authenticationOperationOutput;
                        }
                    }
                }

                if (!manager.IsUserEnable(user))
                {
                    authenticationOperationOutput.Result = AuthenticateOperationResult.Inactive;
                    VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Try login with inactive user");
                }
                else
                {
                    string loggedInUserPassword = manager.GetUserPassword(user.UserId);

                    if (HashingUtility.VerifyHash(password, "", loggedInUserPassword))
                    {
                        SecurityToken securityToken = new SecurityToken
                        {
                            UserId = user.UserId,
                            IssuedAt = DateTime.Now,
                            ExpiresAt = DateTime.Now.Add(s_tokenExpirationInterval)
                        };
                        AddTokenExtensions(securityToken);

                        string encrypted = Common.Cryptography.Encrypt(Common.Serializer.Serialize(securityToken), ConfigurationManager.AppSettings[SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY]);
                        authToken.Token = encrypted;
                        authenticationOperationOutput.Result = AuthenticateOperationResult.Succeeded;
                        authenticationOperationOutput.AuthenticationObject = authToken;

                        dataManager.UpdateLastLogin(user.UserId);
                        VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Login successfully");

                    }
                    else
                    {
                        string loggedInUserTempPassword = manager.GetUserTempPassword(user.UserId);
                        if (HashingUtility.VerifyHash(password, "", loggedInUserTempPassword))
                        {
                            authenticationOperationOutput.Result = AuthenticateOperationResult.ActivationNeeded;
                            VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Try login with activation needed");

                        }
                        else
                        {

                            int addedId;
                            UserFailedLoginManager userFailedLoginManager = new UserFailedLoginManager();
                            userFailedLoginManager.AddUserFailedLogin(new UserFailedLogin
                            {
                                FailedResultId = (int)AuthenticateOperationResult.WrongCredentials,
                                UserId = user.UserId
                            }, out addedId);
                            authenticationOperationOutput.Result = AuthenticateOperationResult.WrongCredentials;
                            VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Try login with wrong credentials");
                        }
                    }
                }

            }
            else
            {
                authenticationOperationOutput.Result = AuthenticateOperationResult.UserNotExists;
                LoggerFactory.GetLogger().WriteError("User '{0}' failed to login", email);
            }

            return authenticationOperationOutput;
        }

        private static void SendNotificationMail(User user, ConfigManager cManager)
        {
            Guid? notificationId = cManager.GetNotificationMailTemplateId();

            if (notificationId.HasValue)
            {
                Dictionary<string, dynamic> mailObjects = new Dictionary<string, dynamic>();
                mailObjects.Add("User", user);
                VRMailManager vrMailManager = new VRMailManager();
                vrMailManager.SendMail(notificationId.Value, mailObjects);
            }
        }

        private void AddTokenExtensions(SecurityToken securityToken)
        {
            foreach (var type in Vanrise.Common.Utilities.GetAllImplementations<SecurityTokenExtensionBehavior>())
            {
                SecurityTokenExtensionBehavior behavior = Activator.CreateInstance(type) as SecurityTokenExtensionBehavior;
                if (behavior == null)
                    throw new Exception(String.Format("behaviorType '{0}' is not of type SecurityTokenExtensionBehavior", type));
                var context = new SecurityTokenExtensionContext { Token = securityToken };
                behavior.AddExtensionsToToken(context);
            }
        }


        public bool IsAllowed(RequiredPermissionSettings requiredPermissions, int userId)
        {
            string requiredPermissionString = RequiredPermissionsToString(requiredPermissions.Entries);
            return IsAllowed(requiredPermissionString, userId);
        }

        public bool IsAllowed(string requiredPermissions, int userId)
        {
            if (requiredPermissions == null)
                return true;

            //Assume that the view is allowed, and start looping until you find an exception that prevents the user from seeing this view
            bool result = true;

            Dictionary<string, List<string>> reqPermissionsDic = GetRequiredPermissionsByNodePath(requiredPermissions);

            PermissionManager manager = new PermissionManager();
            EffectivePermissionsWrapper effectivePermissionsWrapper = manager.GetEffectivePermissions(userId);

            foreach (KeyValuePair<string, List<string>> kvp in reqPermissionsDic)
            {
                result = CheckPermissions(kvp.Key, kvp.Value, effectivePermissionsWrapper.EffectivePermissions, effectivePermissionsWrapper.BreakInheritanceEntities, new HashSet<string>());
                if (!result)
                    break;
            }

            return result;
        }

        public RequiredPermissionSettings MergeRequiredPermissions(List<RequiredPermissionSettings> requiredPermissions)
        {
            Dictionary<Guid, RequiredPermissionEntry> entitiesPermissions = new Dictionary<Guid, RequiredPermissionEntry>();
            foreach (var requiredPerm in requiredPermissions)
            {
                foreach (var entry in requiredPerm.Entries)
                {
                    RequiredPermissionEntry matchEntry = entitiesPermissions.GetOrCreateItem(entry.EntityId, () => new RequiredPermissionEntry { EntityId = entry.EntityId, PermissionOptions = new List<string>() });
                    foreach (var flag in entry.PermissionOptions)
                    {
                        if (!matchEntry.PermissionOptions.Contains(flag))
                            matchEntry.PermissionOptions.Add(flag);
                    }
                }
            }
            return new RequiredPermissionSettings { Entries = entitiesPermissions.Values.ToList() };
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ChangePassword(string oldPassword, string newPassword)
        {
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            UserManager manager = new UserManager();

            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            string validationMessage;
            if (!DoesPasswordMeetRequirement(newPassword, out validationMessage))
            {
                updateOperationOutput.Message = validationMessage;
                return updateOperationOutput;
            }

            if (IsPasswordSame(loggedInUserId, newPassword, out validationMessage))
            {
                updateOperationOutput.Message = validationMessage;
                return updateOperationOutput;
            }

            User currentUser = manager.GetUserbyId(loggedInUserId);
            string currentUserPassword = manager.GetUserPassword(loggedInUserId);

            bool changePasswordActionSucc = false;
            bool oldPasswordIsCorrect = HashingUtility.VerifyHash(oldPassword, "", currentUserPassword);
            string encryptedNewPassword = "";
            if (oldPasswordIsCorrect)
            {
                encryptedNewPassword = HashingUtility.ComputeHash(newPassword, "", null);
                changePasswordActionSucc = dataManager.ChangePassword(loggedInUserId, encryptedNewPassword);
            }

            if (changePasswordActionSucc)
            {
                new UserPasswordHistoryManager().AddPasswordHistory(loggedInUserId, encryptedNewPassword, false);
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }

            return updateOperationOutput;
        }

        public bool IsPasswordSame(int userId, string newPassword, out string validationMessage)
        {
            validationMessage = "";
            ConfigManager configManager = new ConfigManager();
            int maxRecordCount = configManager.GetMaxRecordsCount();

            IEnumerable<UserPasswordHistory> passwordHistoryLogs = new UserPasswordHistoryManager().GetUserPasswordHistoryByUserId(userId, maxRecordCount);
            foreach (var log in passwordHistoryLogs)
            {
                if (HashingUtility.VerifyHash(newPassword, "", log.Password))
                {
                    validationMessage = "Password entered matches old passwords. Please enter a new password";
                    return true;
                }
            }
            return false;
        }

        public string GetCookieName()
        {
            var authServer = GetAuthServer();
            if (authServer != null)
            {
                return authServer.Settings.AuthenticationCookieName;
            }
            else
                return ConfigurationManager.AppSettings["Sec_AuthCookieName"];
        }

        public string GetLoginURL()
        {
            var authServer = GetAuthServer();
            if (authServer != null)
            {
                return String.Format("{0}/Security/Login", authServer.Settings.OnlineURL);
            }
            else
                return null;
        }

        public string GetTokenDecryptionKey()
        {
            var authServer = GetAuthServer();
            if (authServer != null)
            {
                return authServer.Settings.TokenDecryptionKey;
            }
            else
                return ConfigurationManager.AppSettings[SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY];
        }

        CloudAuthServer GetAuthServer()
        {
            CloudAuthServerManager authServerManager = new CloudAuthServerManager();
            var authServer = authServerManager.GetAuthServer();
            if (authServer != null)
            {
                if (authServer.Settings == null)
                    throw new NullReferenceException("authServer.Settings");
                return authServer;
            }
            else
                return null;
        }

        public int GetPasswordComplexityScore(string password)
        {
            int score = -1;
            if (Regex.Match(password, @"[0-9]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[~!@#$%^&*?_~£(){}]", RegexOptions.ECMAScript).Success)
                score++;
            return score;
        }

        public bool DoesPasswordMeetRequirement(string password, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Password cannot be empty";
                return false;
            }
            ConfigManager configManager = new ConfigManager();
            int passwordLength = configManager.GetPasswordLength();
            int maxPasswordLength = configManager.GetMaxPasswordLength();
            var complexity = configManager.GetPasswordComplexity();

            if (password.Trim().Length < passwordLength)
            {
                errorMessage = string.Format("Password must be at least {0} characters.", passwordLength);
                return false;
            }
            if (password.Trim().Length > maxPasswordLength)
            {
                errorMessage = string.Format("Password must be at most {0} characters.", maxPasswordLength);
                return false;
            }
            if (complexity.HasValue && GetPasswordComplexityScore(password) < (int)complexity.Value)
            {
                errorMessage = "Password does not match complexity level.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public PasswordValidationInfo GetPasswordValidationInfo()
        {
            StringBuilder msg = new StringBuilder();
            msg.Append("Password must meet complexity requirements:");
            ConfigManager cManager = new ConfigManager();
            int passwordLength = cManager.GetPasswordLength();
            int maxPasswordLength = cManager.GetMaxPasswordLength();
            var complexity = cManager.GetPasswordComplexity();
            msg.Append(string.Format("<br> - Length must be at least {0} characters.", passwordLength));
            msg.Append(string.Format("<br> - Length must be at most {0} characters.", maxPasswordLength));

            if (complexity.HasValue)
            {
                msg.Append(string.Format("<br> - Passwords must contain characters from {0} of the following four categories:", complexity.Value == PasswordComplexity.Medium ? "two" : "three"));
                msg.Append("<ul><li>Uppercase characters of European languages (A through Z).</li>");
                msg.Append("<li>Lowercase characters of European languages (a through z).</li>");
                msg.Append("<li>Base 10 digits (0 through 9).</li>");
                msg.Append("<li>Nonalphanumeric characters:  ~,!,@,#,$,%,^,&,*,?,_,~,-,£,(,).</li></ul>");
            }
            return new PasswordValidationInfo()
            {
                RequirementsMessage = msg.ToString(),
                RequiredPassword = !cManager.ShouldSendEmailOnNewUser()
            };
        }


        public bool CheckTokenAccess(SecurityToken securityToken, out string errorMessage, out InvalidAccess invalidAccess)
        {

            if (securityToken.ExpiresAt < DateTime.Now)
            {
                errorMessage = "Token Expired";
                invalidAccess = InvalidAccess.TokenExpired;
                return false;
            }
            var authServer = GetAuthServer();
            if (authServer != null)
            {
                if (securityToken.AccessibleCloudApplications == null || !securityToken.AccessibleCloudApplications.Any(app => app.ApplicationId == authServer.Settings.CurrentApplicationId))
                {
                    errorMessage = "You don't have access to this application";
                    invalidAccess = InvalidAccess.UnauthorizeAccess;
                    return false;
                }

                int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
                UserManager userManager = new UserManager();
                User currentUser = userManager.GetUserbyId(loggedInUserId);
                if (!CheckTenantLicenseIfValid(currentUser.TenantId, authServer.Settings.CurrentApplicationId))
                {
                    errorMessage = "License Expired";
                    invalidAccess = InvalidAccess.LicenseExpired;
                    return false;
                }
            }
            errorMessage = null;
            invalidAccess = InvalidAccess.None;
            return true;
        }

        private bool CheckTenantLicenseIfValid(int tenantId, int applicationId)
        {
            TenantManager tenantManager = new TenantManager();
            CloudTenantOutput output = tenantManager.GetCloudTenantOutput(tenantId, applicationId);

            if (output == null)
                return false;

            if (!output.LicenseExpiresOn.HasValue)
                return true;

            if (output.LicenseExpiresOn < DateTime.Now)
                return false;

            return true;
        }

        #endregion

        #region Private Methods

        private bool CheckPermissions(string requiredPath, List<string> requiredFlags, Dictionary<string, Dictionary<string, Flag>> effectivePermissions,
            HashSet<string> breakInheritanceEntities, HashSet<string> allowedFlags)
        {
            bool result = true;

            Dictionary<string, Flag> effectivePermissionFlags;
            if (effectivePermissions.TryGetValue(requiredPath, out effectivePermissionFlags))
            {
                Flag fullControlFlag;
                if (effectivePermissionFlags.TryGetValue("Full Control", out fullControlFlag))
                {
                    if (fullControlFlag == Flag.DENY)
                        return false;
                    else
                    {
                        foreach (var flag in requiredFlags)
                            allowedFlags.Add(flag);
                    }
                }
                else
                {
                    foreach (string requiredFlag in requiredFlags)
                    {
                        Flag effectivePermissionFlag;
                        if (effectivePermissionFlags.TryGetValue(requiredFlag, out effectivePermissionFlag))
                        {
                            if (effectivePermissionFlag == Flag.DENY)
                            {
                                return false;
                            }
                            else
                            {
                                allowedFlags.Add(requiredFlag);
                            }
                        }
                    }
                }
            }

            //The required path might be in one level up, then check it on that level recursively
            int index = requiredPath.LastIndexOf('/');
            if (index > 0 && !breakInheritanceEntities.Contains(requiredPath))
            {
                //Keep looping recursively until you finish trimming the whole string requiredPath
                string oneLevelUp = requiredPath.Remove(index);
                result = CheckPermissions(oneLevelUp, requiredFlags, effectivePermissions, breakInheritanceEntities, allowedFlags);
            }
            else
            {
                //Validation logic
                foreach (string item in requiredFlags)
                {
                    if (!allowedFlags.Contains(item))
                        return false;
                }
            }

            return result;
        }

        internal static string RequiredPermissionsToString(List<RequiredPermissionEntry> requirePermissions)
        {
            if (requirePermissions == null || requirePermissions.Count == 0)
                return null;
            StringBuilder builder = new StringBuilder();
            bool isFirstPermission = true;
            foreach (var p in requirePermissions.OrderBy(itm => itm.EntityId))
            {
                if (!isFirstPermission)
                    builder.Append("&");
                isFirstPermission = false;
                BusinessEntity be = new BusinessEntityManager().GetBusinessEntityById(p.EntityId);
                be.ThrowIfNull("be", p.EntityId);
                string beName = be.Name.Trim();
                builder.Append(beName);
                builder.Append(":");
                bool isFirstOption = true;
                foreach (string s in p.PermissionOptions.Select(itm => itm.Trim()).OrderBy(itm => itm))
                {
                    if (!isFirstOption)
                        builder.Append(",");
                    isFirstOption = false;
                    builder.Append(s);
                }
            }
            return builder.ToString();
        }

        #endregion

        #region Pending Methods

        public bool HasPermissionToActions(string systemActionNames, int userId)
        {
            return IsAllowed(GetRequiredPermissionsFromSystemActionNames(systemActionNames), userId);
        }

        string GetRequiredPermissionsFromSystemActionNames(string systemActionNames)
        {
            string[] systemActionNamesArray = systemActionNames.Split('&');
            var requiredPermissions = new List<string>();

            SystemActionManager systemActionManager = new SystemActionManager();

            foreach (string systemActionName in systemActionNamesArray)
            {
                SystemAction systemAction = systemActionManager.GetSystemAction(systemActionName.Trim());

                //if (systemAction == null)
                //    throw new NullReferenceException(String.Format("System action '{0}' was not found", systemActionName));

                if (systemAction != null && systemAction.RequiredPermissions != null)
                {
                    requiredPermissions.Add(systemAction.RequiredPermissions);
                }
            }

            return (requiredPermissions.Count > 0) ? String.Join("&", requiredPermissions) : null;
        }

        Dictionary<string, List<string>> GetRequiredPermissionsByNodePath(string input)
        {
            var dictionary = new Dictionary<string, List<string>>();
            BusinessEntityNodeManager beNodeManager = new BusinessEntityNodeManager();

            string[] permissionsByNodeNameArray = input.Split('&');

            foreach (string permissionsByNodeNameString in permissionsByNodeNameArray)
            {
                string[] kvp = permissionsByNodeNameString.Split(':');

                if (kvp.Length != 2)
                    throw new FormatException(String.Format("'{0}' is an invalid key value pair", permissionsByNodeNameString));

                if (kvp[0] == String.Empty)
                    throw new FormatException("Node is not defined");

                if (kvp[1] == String.Empty)
                    throw new FormatException("Permissions are not defined");

                string nodePath = beNodeManager.GetBusinessEntityNodePath(kvp[0].Trim());

                if (nodePath == null)
                    throw new NullReferenceException(String.Format("Node '{0}' was not found", kvp[0]));

                List<string> permissions = kvp[1].Split(',').MapRecords(itm => itm.Trim()).ToList();

                List<string> existingPermissions;
                dictionary.TryGetValue(nodePath, out existingPermissions);

                if (existingPermissions != null)
                    existingPermissions.AddRange(permissions);
                else
                    dictionary.Add(nodePath, permissions);
            }

            return dictionary;
        }

        #endregion

        #region Private Classes

        public class SecurityTokenExtensionContext : ISecurityTokenExtensionContext
        {
            public SecurityToken Token
            {
                get;
                set;
            }
        }

        #endregion
    }
}
