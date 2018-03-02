using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerAssignment : Vanrise.Entities.IDateEffectiveSettings
    {
        public long AccountManagerAssignementId { get; set; }

        public Guid AccountManagerAssignementDefinitionId { get; set; }

        public long AccountManagerId { get; set; }

        public string AccountId { get; set; }

        public AccountManagerAssignmentSettings Settings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }

    public class AccountManagerAssignmentSettings
    {
        public AccountManagerAssignmentExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class AccountManagerAssignmentExtendedSettings
    {

    }
}
