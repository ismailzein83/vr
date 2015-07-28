using System;
using System.Collections.Generic;
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

            if (token == null)
                throw new KeyNotFoundException(SecurityContext.SECURITY_TOKEN_NAME + " not found in request header");

            return Common.Serializer.Deserialize<SecurityToken>(Common.TempEncryptionHelper.Decrypt(token));
        }

        #endregion

    }
}
