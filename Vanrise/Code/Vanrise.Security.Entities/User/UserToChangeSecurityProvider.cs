using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserToChangeSecurityProvider
    {
        public int UserId { get; set; }
        public Guid SecurityProviderId { get; set; }
        public string Email { get; set; }
        public bool EnablePasswordExpiration { get; set; }
        public string Password { get; set; }

    }
}
