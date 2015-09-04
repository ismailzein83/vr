using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityContext
    {
        #region Constants

        public const string SECURITY_TOKEN_NAME = "Auth-Token";
        public const string SECURITY_ENCRYPTION_SECRETE_KEY = "EncryptionSecreteKey";

        #endregion

        #region Signleton

        private static SecurityContext _current;

        static SecurityContext()
        {
            _current = new SecurityContext();
        }

        public static SecurityContext Current
        {
            get
            {
                return _current;
            }
        }

        #endregion

        #region Public Methods

        public int GetLoggedInUserId()
        {
            return this.GetSecurityToken().UserId;
        }

        public bool IsAllowed(string requiredPermissions)
        {
            SecurityManager manager = new SecurityManager();
            return manager.IsAllowed(requiredPermissions, this.GetSecurityToken().UserId);
        }

        #endregion

        #region Private Methods

        private SecurityToken GetSecurityToken()
        {
            //TODO: handle the exception Key Not found in case the auth-toekn was null
            string token = HttpContext.Current.Request.Headers[SecurityContext.SECURITY_TOKEN_NAME];
            string decryptedKey = Common.Cryptography.Decrypt(token, ConfigurationManager.AppSettings[SecurityContext.SECURITY_ENCRYPTION_SECRETE_KEY]);
            
            return Common.Serializer.Deserialize<SecurityToken>(decryptedKey);
        }

        #endregion

    }
}
