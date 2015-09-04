using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserInfo
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public UserStatus Status { get; set; }
    }
}
