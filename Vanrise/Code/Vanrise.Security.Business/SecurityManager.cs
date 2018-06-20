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
        static UserManager s_userManager = new UserManager();
        static ConfigManager s_configManager = new ConfigManager();

        #region Public Methods

        public AuthenticateOperationOutput<AuthenticationToken> Authenticate(Guid securityProviderId, SecurityProviderAuthenticationPayload payload)
        {
            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(securityProviderId);
            securityProvider.ThrowIfNull("securityProvider", securityProviderId);

            SecurityProviderAuthenticateContext context = new SecurityProviderAuthenticateContext() { Payload = payload };
            SecurityProviderAuthenticateResult result = securityProvider.Settings.ExtendedSettings.Authenticate(context);

            AuthenticateOperationOutput<AuthenticationToken> authenticationOperationOutput = new AuthenticateOperationOutput<AuthenticationToken>();
            authenticationOperationOutput.Result = AuthenticateOperationResult.Failed;
            authenticationOperationOutput.AuthenticationObject = null;

            if (result != SecurityProviderAuthenticateResult.Succeeded)
            {
                authenticationOperationOutput.Message = context.FailureMessage;
                switch (result)
                {
                    case SecurityProviderAuthenticateResult.ActivationNeeded:
                        authenticationOperationOutput.Result = AuthenticateOperationResult.ActivationNeeded;
                        break;
                    case SecurityProviderAuthenticateResult.UserNotExists:
                        authenticationOperationOutput.Result = AuthenticateOperationResult.UserNotExists;
                        break;
                    case SecurityProviderAuthenticateResult.WrongCredentials:
                        authenticationOperationOutput.Result = AuthenticateOperationResult.WrongCredentials;
                        break;
                    case SecurityProviderAuthenticateResult.PasswordExpired:
                        authenticationOperationOutput.Result = AuthenticateOperationResult.PasswordExpired;
                        break;
                    case SecurityProviderAuthenticateResult.Inactive:
                        authenticationOperationOutput.Result = AuthenticateOperationResult.Inactive;
                        break;
                    case SecurityProviderAuthenticateResult.Failed:
                        authenticationOperationOutput.Result = AuthenticateOperationResult.Failed;
                        break;
                    default: throw new NotSupportedException(string.Format("SecurityProviderAuthenticateResult {0} not supported.", result));
                }
                return authenticationOperationOutput;
            }

            User user = context.AuthenticatedUser;
            user.ThrowIfNull("user");
            if (user.IsSystemUser)
                throw new Exception("Can not login using System Account");

            if (user.SecurityProviderId != securityProviderId)
            {
                authenticationOperationOutput.Result = AuthenticateOperationResult.WrongCredentials;
                return authenticationOperationOutput;
            }

            if (!new UserManager().IsUserEnable(user))
            {
                VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Try login with inactive user");
                authenticationOperationOutput.Result = AuthenticateOperationResult.Inactive;
                return authenticationOperationOutput;
            }

            AuthenticationToken authToken = CreateAuthenticationToken(user);
            authToken.PasswordExpirationDaysLeft = context.PasswordExpirationDaysLeft;
            authenticationOperationOutput.Result = AuthenticateOperationResult.Succeeded;
            authenticationOperationOutput.AuthenticationObject = authToken;
            int lastModifiedBy = user.UserId;
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            dataManager.UpdateLastLogin(user.UserId, lastModifiedBy);
            VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Login successfully");

            return authenticationOperationOutput;
        }

        public AuthenticateOperationOutput<AuthenticationToken> Authenticate(string email, string password)
        {
            EmailPasswordSecurityProviderAuthenticationPayload payload = new EmailPasswordSecurityProviderAuthenticationPayload() { Email = email, Password = password };
            Guid defaultSecurityProviderId = new Security.Business.ConfigManager().GetDefaultSecurityProviderId();
            return Authenticate(defaultSecurityProviderId, payload);
        }

        public ApplicationRedirectOutput RedirectToApplication(string applicationURL)
        {
            int userId = SecurityContext.Current.GetLoggedInUserId();
            User user = new UserManager().GetUserbyId(userId);

            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(user.SecurityProviderId);
            securityProvider.ThrowIfNull("securityProvider", user.SecurityProviderId);

            SecurityToken currentUserToken;
            if (!SecurityContext.TryGetSecurityToken(out currentUserToken))
                return null;

            string encryptedToken = Common.Cryptography.Encrypt(Common.Serializer.Serialize(currentUserToken), GetLocalTokenDecryptionKey());

            SecurityProviderGetApplicationRedirectInputContext context = new SecurityProviderGetApplicationRedirectInputContext() { Token = encryptedToken, Email = user.Email };
            ApplicationRedirectInput applicationRedirectInput = securityProvider.Settings.ExtendedSettings.GetApplicationRedirectInput(context);

            return Vanrise.Common.VRWebAPIClient.Post<ApplicationRedirectInput, ApplicationRedirectOutput>(applicationURL, "/api/VR_Sec/Security/TryGenerateToken", applicationRedirectInput);
        }

        public ApplicationRedirectOutput TryGenerateToken(ApplicationRedirectInput input)
        {
            User user = new UserManager().GetUserbyEmail(input.Email);
            if (user == null)
                return null;

            if (user.IsSystemUser)
                return null;

            if (!new UserManager().IsUserEnable(user))
            {
                VRActionLogger.Current.LogObjectCustomAction(UserManager.UserLoggableEntity.Instance, "Login", false, user, "Try login with inactive user");
                return null;
            }

            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(user.SecurityProviderId);
            securityProvider.ThrowIfNull("securityProvider", user.SecurityProviderId);

            SecurityProviderValidateSecurityTokenContext context = new SecurityProviderValidateSecurityTokenContext() { Token = input.Token, ApplicationId = input.ApplicationId };
            bool isValid = securityProvider.Settings.ExtendedSettings.ValidateSecurityToken(context);
            if (!isValid)
                return null;

            ApplicationRedirectOutput output = new ApplicationRedirectOutput()
            {
                CookieName = GetCookieName(),
                AuthenticationToken = CreateAuthenticationToken(user)
            };

            return output;
        }

        public bool ValidateSecurityToken(ValidateSecurityTokenInput input)
        {
            if (input.ApplicationId.HasValue)
            {
                RegisteredApplication registeredApplication = new RegisteredApplicationManager().GetRegisteredApplicationbyId(input.ApplicationId.Value);
                if (registeredApplication == null)
                    return false;

                string applicationURL = registeredApplication.URL;
                if (string.IsNullOrEmpty(applicationURL))
                    return false;

                ValidateSecurityTokenInput validateSecurityTokenInput = new ValidateSecurityTokenInput() { Token = input.Token };
                return Vanrise.Common.VRWebAPIClient.Post<ValidateSecurityTokenInput, bool>(applicationURL, "/api/VR_Sec/Security/ValidateSecurityToken", validateSecurityTokenInput);
            }
            else
            {
                string decryptionKey = GetTokenDecryptionKey();
                string decryptedToken = Common.Cryptography.Decrypt(input.Token, decryptionKey);

                SecurityToken securityToken = Common.Serializer.Deserialize<SecurityToken>(decryptedToken);
                if (securityToken == null)
                    return false;

                if (securityToken.ExpiresAt < DateTime.Now)
                    return false;

                return true;
            }
        }

        public bool TryRenewCurrentAuthenticationToken(out AuthenticationToken newAuthenticationToken)
        {
            SecurityToken currentUserToken;
            if (SecurityContext.TryGetSecurityToken(out currentUserToken))
            {
                if (currentUserToken.ExpiresAt >= DateTime.Now)
                {
                    newAuthenticationToken = CreateAuthenticationToken(currentUserToken.UserId);
                    return true;
                }
            }
            newAuthenticationToken = null;
            return false;
        }

        public AuthenticationToken CreateAuthenticationToken(int userId)
        {
            var user = s_userManager.GetUserbyId(userId);
            user.ThrowIfNull("user", userId);
            return CreateAuthenticationToken(user);
        }

        public AuthenticationToken CreateAuthenticationToken(User user)
        {
            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(user.SecurityProviderId);
            AuthenticationToken authToken = new AuthenticationToken();
            authToken.TokenName = SecurityContext.SECURITY_TOKEN_NAME;
            authToken.Token = null;
            authToken.UserName = user.Email;
            authToken.SecurityProviderId = user.SecurityProviderId;
            authToken.SupportPasswordManagement = securityProvider.Settings.ExtendedSettings.SupportPasswordManagement;
            authToken.UserDisplayName = user.Name;
            authToken.PhotoFileId = user.Settings != null ? user.Settings.PhotoFileId : null;
            int sessionExpirationInMinutes = s_configManager.GetSessionExpirationInMinutes();
            authToken.ExpirationIntervalInMinutes = sessionExpirationInMinutes;
            authToken.ExpirationIntervalInSeconds = sessionExpirationInMinutes * 60;

            SecurityToken securityToken = new SecurityToken
            {
                UserId = user.UserId,
                IssuedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(sessionExpirationInMinutes)
            };
            AddTokenExtensions(securityToken);

            string encrypted = Common.Cryptography.Encrypt(Common.Serializer.Serialize(securityToken), GetLocalTokenDecryptionKey());
            authToken.Token = encrypted;
            return authToken;
        }

        public static void SendNotificationMail(User user, ConfigManager cManager)
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
            string requiredPermissionString = null;
            if (requiredPermissions != null)
                requiredPermissionString = RequiredPermissionsToString(requiredPermissions.Entries);
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

        public Vanrise.Entities.UpdateOperationOutput<object> ChangeExpiredPassword(string email, string oldPassword, string newPassword)
        {
            User user = s_userManager.GetUserbyEmail(email);
            return ChangePassword(user.UserId, oldPassword, newPassword, true);
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ChangePassword(string oldPassword, string newPassword)
        {
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            return ChangePassword(loggedInUserId, oldPassword, newPassword, false);
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ChangePassword(int userId, string oldPassword, string newPassword, bool passwordExpired)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            UserManager manager = new UserManager();

            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            User user = new UserManager().GetUserbyId(userId);
            SecurityProvider securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(user.SecurityProviderId);
            securityProvider.ThrowIfNull("securityProvider", user.SecurityProviderId);

            SecurityProviderChangePasswordContext changePasswordContext = new SecurityProviderChangePasswordContext() { User = user, NewPassword = newPassword, OldPassword = oldPassword, PasswordExpired = passwordExpired };
            bool updateActionSucc = securityProvider.Settings.ExtendedSettings.ChangePassword(changePasswordContext);

            if (updateActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }
            else
            {
                updateOperationOutput.ShowExactMessage = changePasswordContext.ShowExactMessage;
                updateOperationOutput.Message = changePasswordContext.ValidationMessage;
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
                    validationMessage = string.Format("Password entered matches previous {0} history passwords. Please enter a new password", maxRecordCount);
                    return true;
                }
            }
            return false;
        }

        public string GetCookieName()
        {
            return GetLocalCookieName();
        }

        public string GetLoginURL()
        {
            return null;
        }
        public string GetTokenDecryptionKey()
        {
            return GetLocalTokenDecryptionKey();
        }

        static string s_localTokenDecryptionKey;

        static Object s_GetLocalTokenDecryptionKey_LockObj = new object();
        private string GetLocalTokenDecryptionKey()
        {
            if (s_localTokenDecryptionKey == null)
            {
                lock (s_GetLocalTokenDecryptionKey_LockObj)
                {
                    if (s_localTokenDecryptionKey == null)
                    {
                        var dataManager = SecurityDataManagerFactory.GetDataManager<IEncryptionKeyDataManager>();
                        string keyToInsertIfNotExists = Guid.NewGuid().ToString();
                        s_localTokenDecryptionKey = string.Format("{0}_{1}", ConfigurationManager.AppSettings[SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY], dataManager.InsertIfNotExistsAndGetEncryptionKey(keyToInsertIfNotExists));
                    }
                }
            }
            return s_localTokenDecryptionKey;
        }

        static string s_localCookieName;

        static Object s_localCookieName_LockObj = new object();
        private string GetLocalCookieName()
        {
            if (s_localCookieName == null)
            {
                lock (s_localCookieName_LockObj)
                {
                    if (s_localCookieName == null)
                    {
                        var dataManager = SecurityDataManagerFactory.GetDataManager<ICookieNameDataManager>();
                        string cookieNameInsertIfNotExists = String.Concat("Vanrise_AccessCookie-", Guid.NewGuid().ToString());
                        s_localCookieName = dataManager.InsertIfNotExistsAndGetCookieName(cookieNameInsertIfNotExists);
                    }
                }
            }
            return s_localCookieName;
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


        public bool GetExactExceptionMessage()
        {
            ConfigManager configManager = new ConfigManager();
            return configManager.GetExactExceptionMessage();
        }

        public PasswordValidationInfo GetPasswordValidationInfo(Guid? securityProviderId)
        {
            SecurityProvider securityProvider;
            if (securityProviderId.HasValue)
            {
                securityProvider = new SecurityProviderManager().GetSecurityProviderbyId(securityProviderId.Value);
                securityProvider.ThrowIfNull("securityProvider", securityProviderId.Value);
            }
            else
            {
                securityProvider = new SecurityProviderManager().GetDefaultSecurityProvider();
                securityProvider.ThrowIfNull("default securityProvider");
            }

            return securityProvider.Settings.ExtendedSettings.GetPasswordValidationInfo(new SecurityProviderGetPasswordValidationInfoContext());
        }

        public PasswordValidationInfo GetRemotePasswordValidationInfo(Guid connectionId, Guid? securityProviderId)
        {
            VRConnectionManager connectionManager = new VRConnectionManager();
            var vrConnection = connectionManager.GetVRConnection<VRInterAppRestConnection>(connectionId);
            VRInterAppRestConnection connectionSettings = vrConnection.Settings as VRInterAppRestConnection;
            string args = securityProviderId.HasValue ? string.Format("?securityProviderId={0}", securityProviderId) : string.Empty;

            return connectionSettings.Get<PasswordValidationInfo>(string.Format("/api/VR_Sec/Security/GetPasswordValidationInfo", args));
        }

        public bool CheckTokenAccess(SecurityToken securityToken, out string errorMessage, out InvalidAccess invalidAccess)
        {
            if (securityToken.ExpiresAt < DateTime.Now)
            {
                errorMessage = "Token Expired";
                invalidAccess = InvalidAccess.TokenExpired;
                return false;
            }

            errorMessage = null;
            invalidAccess = InvalidAccess.None;
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
