using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class AuthenticationToken
    {
        public string TokenName { get; set; }

        public int ExpirationIntervalInMinutes { get; set; }

        public string Username { get; set; }

        public string UserDisplayName { get; set; }

        public string Token { get; set; }
    }

    public class SecurityToken
    {
        public int UserId { get; set; }
    }
}
