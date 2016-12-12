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

        public int TenantId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime? LastLogin { get; set; }

        public string Description { get; set; }

        public DateTime? EnabledTill { get; set; }
    }
}
