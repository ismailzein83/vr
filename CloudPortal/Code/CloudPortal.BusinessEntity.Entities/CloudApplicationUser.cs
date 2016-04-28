using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace CloudPortal.BusinessEntity.Entities
{
    public class CloudApplicationUser
    {
        public int ApplicationId { get; set; }

        public int UserId { get; set; }

        public CloudApplicationUserSettings Settings { get; set; }
    }

    public class CloudApplicationUserSettings
    {
        public UserStatus Status { get; set; }

        public string Description { get; set; }
    }
}
