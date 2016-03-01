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

            if(user != null)
            {
                int expirationPeriodInMinutes = 1440;

                AuthenticationToken authToken = new AuthenticationToken();
                authToken.TokenName = SecurityContext.SECURITY_TOKEN_NAME;
                authToken.Token = null;
                authToken.UserName = user.Email;
                authToken.UserDisplayName = user.Name;
                authToken.ExpirationIntervalInMinutes = expirationPeriodInMinutes;

                string loggedInUserPassword = manager.GetUserPassword(user.UserId);

                if (user.Status == UserStatus.Inactive)
                {
                    authenticationOperationOutput.Result = AuthenticateOperationResult.Inactive;
                }
                else if (!HashingUtility.VerifyHash(password, "", loggedInUserPassword))
                {
                    authenticationOperationOutput.Result = AuthenticateOperationResult.WrongCredentials;
                }
                else
                {
                    SecurityToken userInfo = new SecurityToken
                    {
                        UserId = user.UserId,
                        IssuedAt = DateTime.Now,
                        ExpiresAt = DateTime.Now.AddMinutes(expirationPeriodInMinutes)
                    };

                    string encrypted = Common.Cryptography.Encrypt(Common.Serializer.Serialize(userInfo), ConfigurationManager.AppSettings[SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY]);
                    authToken.Token = encrypted;
                    authenticationOperationOutput.Result = AuthenticateOperationResult.Succeeded;
                    authenticationOperationOutput.AuthenticationObject = authToken;

                    dataManager.UpdateLastLogin(user.UserId);
                }

            }
            else
            {
                authenticationOperationOutput.Result = AuthenticateOperationResult.UserNotExists;
            }

            return authenticationOperationOutput;
        }

        public bool IsAllowed(string requiredPermissions, int userId)
        {
            if (requiredPermissions == null)
                return true;

            //Assume that the view is allowed, and start looping until you find an exception that prevents the user from seeing this view
            bool result = true;

            PermissionManager manager = new PermissionManager();
            EffectivePermissionsWrapper effectivePermissionsWrapper = manager.GetEffectivePermissions(userId);

            Dictionary<string, List<string>> reqPermissionsDic = GetRequiredPermissionsByNodePath(requiredPermissions);

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
                
                if (systemAction == null)
                    throw new NullReferenceException(String.Format("System action '{0}' was not found", systemActionName));

                if (systemAction.RequiredPermissions != null)
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
    }
}
