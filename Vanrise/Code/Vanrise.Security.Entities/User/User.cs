using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public enum UserStatus {
        [Description("Active")]
        Active = 1,
        [Description("Inactive")]
        Inactive = 0,
        [Description("Locked")]
        Locked = 2
    }

    public class User
    {
        public int UserId { get; set; }

        public int TenantId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime? LastLogin { get; set; }

        public string Description { get; set; }

        public DateTime? EnabledTill { get; set; }
        public DateTime? DisabledTill { get; set; }

        public Dictionary<string, Object> ExtendedSettings { get; set; }

        public UserSetting Settings { get; set; }

        public bool IsSystemUser { get; set; }
    }
    public class UserSetting
    {
        public Guid? LanguageId { get; set; }

        public long? PhotoFileId { get; set; }
    }
}
