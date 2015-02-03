using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;
using SecurityEssentials;

namespace TOne.Business
{
    public class SecurityManager
    {
        public AuthenticationOutput Authenticate(string Email, string password)
        {
            //return new AuthenticationOutput
            //{
            //    Result = AuthenticationResult.Succeeded,
            //    User = new User
            //    {
            //        UserId = 1,
            //        Username = username
            //    }
            //};

            SecurityEssentials.User user = SecurityEssentials.User.Authenticate(Email, SecurityEssentials.PasswordEncryption.Encode(password));

            return new AuthenticationOutput
            {
                Result = user.IsAuthenticated ? AuthenticationResult.Succeeded : AuthenticationResult.Failed,
                User = user
            };
        }
    }
}
