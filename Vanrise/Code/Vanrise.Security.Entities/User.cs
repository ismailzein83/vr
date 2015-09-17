using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum UserStatus { Active = 1, Inactive = 0 }

    public class User
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public DateTime? LastLogin { get; set; }

        public string Description { get; set; }

        public UserStatus Status { get; set; }
    }
}
