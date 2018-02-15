using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vanrise.Common;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityContext : ISecurityContext
    {
        #region Constants/Local Variables

        public const string SECURITY_TOKEN_NAME = "Auth-Token";
        public const string SECURITY_ENCRYPTION_SECRETE_KEY = "EncryptionSecretKey";

        [ThreadStatic]
        static int? s_currentContextUserId;

        #endregion

        #region Singleton

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
            if (s_currentContextUserId.HasValue)
                return s_currentContextUserId.Value;
            else
                return this.GetSecurityToken().UserId;
        }

        public bool TryGetLoggedInUserId(out int? userId)
        {
            if (s_currentContextUserId.HasValue)
            {
                userId = s_currentContextUserId.Value;
                return true;
            }
            else
            {
                SecurityToken securityToken;
                if (TryGetSecurityToken(out securityToken))
                {
                    userId = securityToken.UserId;
                    return true;
                }
                else
                {
                    userId = null;
                    return false;
                }
            }
        }
        
        public bool IsAllowed(RequiredPermissionSettings requiredPermissions)
        {
            SecurityManager manager = new SecurityManager();
            return IsAllowed(requiredPermissions, GetLoggedInUserId());
        }
        public bool IsAllowed(RequiredPermissionSettings requiredPermissions , int userId)
        {
            SecurityManager manager = new SecurityManager();
            return manager.IsAllowed(requiredPermissions, userId);
        }

        public bool IsAllowed(string requiredPermissions)
        {
            SecurityManager manager = new SecurityManager();
            return manager.IsAllowed(requiredPermissions, GetLoggedInUserId());
        }

        public bool IsAllowed(string requiredPermissions , int userId)
        {
            SecurityManager manager = new SecurityManager();
            return manager.IsAllowed(requiredPermissions, userId);
        }

        public bool HasPermissionToActions(string systemActionNames)
        {
            SecurityManager manager = new SecurityManager();
            return manager.HasPermissionToActions(systemActionNames, GetLoggedInUserId());
        }

        public void SetContextUserId(int userId)
        {
            SecurityToken securityToken;
            if (TryGetSecurityToken(out securityToken))
            {
                if (securityToken.UserId == userId)
                    return;
                else
                    throw new InvalidOperationException("Current context has already a security token. It is not possible to set User Id");
            }
            s_currentContextUserId = userId;
        }

        #endregion

        #region Private Methods

        private SecurityToken GetSecurityToken()
        {
            SecurityToken securityToken;
            if (!TryGetSecurityToken(out securityToken))
                throw new Exception("SecurityToken is not available in current context.");
            return securityToken;
        }

        public static bool TryGetSecurityToken(out SecurityToken securityToken)
        {
            //TODO: handle the exception Key Not found in case the auth-toekn was null
            try
            {
                securityToken = null;
                if (HttpContext.Current == null)
                    return false;
                string token = null;
                if (HttpContext.Current.Request.Headers[SecurityContext.SECURITY_TOKEN_NAME] != null)
                    token = HttpContext.Current.Request.Headers[SecurityContext.SECURITY_TOKEN_NAME];
                else if (HttpContext.Current.Request.Params[SECURITY_TOKEN_NAME] != null)
                    token = HttpUtility.HtmlDecode(HttpContext.Current.Request.Params[SECURITY_TOKEN_NAME]);
                else
                {
                    if (HttpContext.Current.Request.Cookies != null)
                    {
                        var cookies = HttpContext.Current.Request.Cookies;
                        foreach (var cookieKey in cookies.AllKeys)
                        {
                            if (cookieKey.StartsWith("Vanrise_AccessCookie"))
                            {
                                var cookieValue = System.Web.HttpUtility.UrlDecode(cookies[cookieKey].Value);
                                AuthenticationToken authenticationToken = Serializer.Deserialize<AuthenticationToken>(cookieValue);
                                token = authenticationToken.Token;
                                break;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(token))
                {
                    string decryptionKey = (new SecurityManager()).GetTokenDecryptionKey();
                    string decryptedToken = Common.Cryptography.Decrypt(token, decryptionKey);

                    securityToken = Common.Serializer.Deserialize<SecurityToken>(decryptedToken);
                    return true;
                }
                else
                {
                    securityToken = null;
                    return false;
                }
            }
            catch(Exception ex)
            {
                LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw;
            }
        }

        #endregion
    }
}
