using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityManager
    {
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

                int expirationPeriodInMinutes = 1440;

                AuthenticationToken authToken = new AuthenticationToken();
                authToken.TokenName = SecurityContext.SECURITY_TOKEN_NAME;
                authToken.Token = null;
                authToken.UserName = user.Email;
                authToken.UserDisplayName = user.Name;
                authToken.ExpirationIntervalInMinutes = expirationPeriodInMinutes;

                if (!manager.IsUserEnable(user))
                {
                    authenticationOperationOutput.Result = AuthenticateOperationResult.Inactive;
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
                            ExpiresAt = DateTime.Now.AddMinutes(expirationPeriodInMinutes)
                        };
                        AddTokenExtensions(securityToken);

                        string encrypted = Common.Cryptography.Encrypt(Common.Serializer.Serialize(securityToken), ConfigurationManager.AppSettings[SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY]);
                        authToken.Token = encrypted;
                        authenticationOperationOutput.Result = AuthenticateOperationResult.Succeeded;
                        authenticationOperationOutput.AuthenticationObject = authToken;

                        dataManager.UpdateLastLogin(user.UserId);
                    }
                    else
                    {
                        string loggedInUserTempPassword = manager.GetUserTempPassword(user.UserId);
                        if (HashingUtility.VerifyHash(password, "", loggedInUserTempPassword))
                        {
                            authenticationOperationOutput.Result = AuthenticateOperationResult.ActivationNeeded;
                        }

                        else
                        {
                            authenticationOperationOutput.Result = AuthenticateOperationResult.WrongCredentials;
                        }
                    }
                }

            }
            else
            {
                authenticationOperationOutput.Result = AuthenticateOperationResult.UserNotExists;
            }

            return authenticationOperationOutput;
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
            string requiredPermissionString = RequiredPermissionsToString(requiredPermissions);
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

        public Vanrise.Entities.UpdateOperationOutput<object> ChangePassword(string oldPassword, string newPassword)
        {
            int loggedInUserId = SecurityContext.Current.GetLoggedInUserId();
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            UserManager manager = new UserManager();

            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
            updateOperationOutput.UpdatedObject = null;

            User currentUser = manager.GetUserbyId(loggedInUserId);
            string currentUserPassword = manager.GetUserPassword(loggedInUserId);

            bool changePasswordActionSucc = false;
            bool oldPasswordIsCorrect = HashingUtility.VerifyHash(oldPassword, "", currentUserPassword);

            if (oldPasswordIsCorrect)
            {
                string encryptedNewPassword = HashingUtility.ComputeHash(newPassword, "", null);
                changePasswordActionSucc = dataManager.ChangePassword(loggedInUserId, encryptedNewPassword);
            }

            if (changePasswordActionSucc)
            {
                updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Succeeded;
            }

            return updateOperationOutput;
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
                    errorMessage = "You dont have access to this application";
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

        private string RequiredPermissionsToString(RequiredPermissionSettings requirePermissions)
        {
            if (requirePermissions == null ||  requirePermissions.Entries.Count == 0 )
                return null;
            string rp = "";
            foreach (var p in requirePermissions.Entries)
            {
                BusinessEntity be = new BusinessEntityManager().GetBusinessEntityById(p.EntityId);
                string beName = be.Name;
                rp +=  String.Format("{0}: ", beName);
                foreach (string s in p.PermissionOptions)
                {
                    rp += String.Format("{0}, ", s);
                }
                rp =  rp.Remove(rp.Length - 2,2);
                rp += String.Format("&");

            }
            return rp.Remove(rp.Length - 1, 1);
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
