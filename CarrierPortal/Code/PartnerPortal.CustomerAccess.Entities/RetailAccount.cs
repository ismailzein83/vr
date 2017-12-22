using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.CustomerAccess.Entities
{
    public class RetailAccountSettings
    {
        public long AccountId { get; set; }
        public Guid AccountBEDefinitionId { get; set; }

    }

    public class RetailAccount
    {
        public long AccountId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public int TenantId { get; set; }

        public string Description { get; set; }

        public DateTime? EnabledTill { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
    }
    public class RetailAccountToUpdate
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
