using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.Business
{
    public class SecurityManager
    {
        public AuthenticationOutput Authenticate(string username, string password)
        {
            return new AuthenticationOutput
            {
                Result = AuthenticationResult.Succeeded,
                User = new User
                {
                    UserId = 1,
                    Username = username
                }
            };
        }
    }
}
