using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;

namespace Vanrise.AccountManager.Business
{
    public class AccountManagerAssignmentManager
    {
        public void AssignAccountManagerToAccounts(AssignAccountManagerToAccountsInput input)
        {
            throw new NotImplementedException();
        }

        public void UpdateAccountManagerAssignment(UpdateAccountManagerAssignmentInput input)
        {
            throw new NotImplementedException();
        }

        public bool IsAccountAssignableToAccountManager(Guid assignmentDefinitionId, string accountId)
        {
            throw new NotImplementedException();
        }

        public AccountManagerAssignment GetAccountAssignment(Guid assignmentDefinitionId, string accountId, DateTime effectiveOn)
        {
            throw new NotImplementedException();
        }
    }

    public class AssignAccountManagerToAccountsInput
    {
        public Guid AccountManagerAssignementDefinitionId { get; set; }

        public long AccountManagerId { get; set; }

        public List<AssignAccountManagerToAccountSetting> Accounts { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        ///// <summary>
        ///// later we can implement, for now stop if anyone is invalid
        ///// </summary>
        //public bool ContinueIfInvalid { get; set; }
    }

    public class AssignAccountManagerToAccountSetting
    {
        public string AccountId { get; set; }

        public AccountManagerAssignmentSettings AssignementSettings { get; set; }
    }

    public class AssignAccountManagerToAccountsOutput
    {

    }

    public class UpdateAccountManagerAssignmentInput
    {
        public long AccountManagerAssignmentId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public AccountManagerAssignmentSettings AssignementSettings { get; set; }
    }
}
