using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class UserToAdd
    {
        public int TenantId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? EnabledTill { get; set; }

        public List<int> GroupIds { get; set; }

        public string Password { get; set;  }

        public Dictionary<string, Object> ExtendedSettings { get; set; }

        public long? PhotoFileId { get; set; }

        public bool EnablePasswordExpiration { get; set; }

    }
}
