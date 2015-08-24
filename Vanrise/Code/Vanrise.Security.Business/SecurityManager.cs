using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Data;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityManager
    {
        #region Public Methods

        public AuthenticationToken Authenticate(string email, string password)
        {
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            User user = dataManager.GetUserbyEmail(email);

            AuthenticationToken result = new AuthenticationToken();
            result.TokenName = SecurityContext.SECURITY_TOKEN_NAME;
            result.Token = null;
            result.UserName = user.Email;
            result.UserDisplayName = user.Name;

            if (user.Status != UserStatus.Inactive && user.Password == password)
            {
                SecurityToken userInfo = new SecurityToken
                {
                    UserId = user.UserId
                };
                string encrypted = Common.TempEncryptionHelper.Encrypt(Common.Serializer.Serialize(userInfo));
                result.Token = encrypted;
            }

            return result;
        }

        public bool IsAllowed(string requiredPermissions, int userId)
        {
            //Assume that the view is allowed, and start looping until you find an exception that prevents the user from seeing this view
            bool result = true;

            PermissionManager manager = new PermissionManager();
            EffectivePermissionsWrapper effectivePermissionsWrapper = manager.GetEffectivePermissions(userId);

            Dictionary<string, List<string>> reqPermissionsDic = this.ParseRequiredPermissionsString(requiredPermissions);

            foreach (KeyValuePair<string, List<string>> kvp in reqPermissionsDic)
            {
                result = CheckPermissions(kvp.Key, kvp.Value, effectivePermissionsWrapper.EffectivePermissions, effectivePermissionsWrapper.BreakInheritanceEntities, new HashSet<string>());
                if (!result)
                    break;
            }

            return result;
        }

        public Vanrise.Entities.UpdateOperationOutput<object> ChangePassword(int userId, string oldPassword, string newPassword)
        {           
            IUserDataManager dataManager = SecurityDataManagerFactory.GetDataManager<IUserDataManager>();
            Vanrise.Entities.UpdateOperationOutput<object> updateOperationOutput = new Vanrise.Entities.UpdateOperationOutput<object>();
            bool resetActionSucc = dataManager.ChangePassword(userId , oldPassword,newPassword); 
            updateOperationOutput.Result = Vanrise.Entities.UpdateOperationResult.Failed;
        //     updateOperationOutput.UpdatedObject = null;

             if (resetActionSucc)
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


        private Dictionary<string, List<string>> ParseRequiredPermissionsString(string value)
        {
            Dictionary<string, List<string>> requiredPermissions = null;

            if (value != null)
            {
                requiredPermissions = new Dictionary<string, List<string>>();

                string[] arrayOfPermissions = value.Split('|');

                foreach (string permission in arrayOfPermissions)
                {
                    string[] keyValuesArray = permission.Split(':');
                    List<string> flags = new List<string>();
                    foreach (string flag in keyValuesArray[1].Split(','))
                    {
                        flags.Add(flag.Trim());
                    }

                    requiredPermissions.Add(keyValuesArray[0].Trim(), flags);
                }
            }

            return requiredPermissions;
        }

        #endregion
    }
}
