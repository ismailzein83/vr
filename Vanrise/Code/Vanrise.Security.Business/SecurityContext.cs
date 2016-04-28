﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Vanrise.Security.Entities;

namespace Vanrise.Security.Business
{
    public class SecurityContext : ISecurityContext
    {
        #region Constants

        public const string SECURITY_TOKEN_NAME = "Auth-Token";
        public const string SECURITY_ENCRYPTION_SECRETE_KEY = "EncryptionSecretKey";

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
            return manager.IsAllowed(requiredPermissions, GetLoggedInUserId());
        }

        public bool HasPermissionToActions(string systemActionNames)
        {
            SecurityManager manager = new SecurityManager();
            return manager.HasPermissionToActions(systemActionNames, GetLoggedInUserId());
        }

        #endregion

        #region Private Methods

        private SecurityToken GetSecurityToken()
        {
            //TODO: handle the exception Key Not found in case the auth-toekn was null
            string token = null;
            if (HttpContext.Current.Request.Headers[SecurityContext.SECURITY_TOKEN_NAME] != null)
                token = HttpContext.Current.Request.Headers[SecurityContext.SECURITY_TOKEN_NAME];
            else if (HttpContext.Current.Request.Params[SECURITY_TOKEN_NAME] != null) 
                token =  HttpUtility.HtmlDecode(HttpContext.Current.Request.Params[SECURITY_TOKEN_NAME]);
            string decryptionKey = (new SecurityManager()).GetTokenDecryptionKey();
            string decryptedToken = Common.Cryptography.Decrypt(token, decryptionKey);
            
            return Common.Serializer.Deserialize<SecurityToken>(decryptedToken);
        }

        #endregion
    }
}
