using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class CloudApplicationUser
    {
        public User User { get; set; }

        public UserStatus Status { get; set; }

        public string Description { get; set; }
    }
}
