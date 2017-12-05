using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountManager.Entities
{
    public class AccountManagerAssignmentsInfo
    {
        public AccountManagerAssignmentDefinition AssignmentDefinition { get; set; }
        public Guid AccountManagerDefinitionId { get; set; }
    }
}
