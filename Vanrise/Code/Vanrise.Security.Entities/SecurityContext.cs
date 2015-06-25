using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Vanrise.Security.Entities
{
    public class SecurityContext
    {
        public const string SECURITY_TOKEN_NAME = "Auth-Token";

        public static SecurityToken GetSecurityToken()
        {
            string token = HttpContext.Current.Request.Headers[SecurityContext.SECURITY_TOKEN_NAME];

            if (token == null)
                throw new KeyNotFoundException(SecurityContext.SECURITY_TOKEN_NAME + " not found in request header");

            return Common.Serializer.Deserialize<SecurityToken>(Common.TempEncryptionHelper.Decrypt(token));
        }
    }
}
