using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerAssignmentDefinition
    {
        public Guid AccountPackageAssignementDefinitionId { get; set; }

        public AccountManagerAssignmentDefinitionSettings Settings { get; set; }
    }

    public abstract class AccountManagerAssignmentDefinitionSettings
    {
        public abstract string GetAccountName(string accountId);
    }
}
