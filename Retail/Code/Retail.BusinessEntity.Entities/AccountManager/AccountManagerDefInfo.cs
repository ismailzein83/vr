using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Business;
using Vanrise.AccountManager.Entities;
using Vanrise.GenericData.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class AccountManagerDefInfo
    {
        public Guid AccountManagerDefinitionId { get; set; }
        public AccountManagerBEDefinitionSettings AccountManagerDefinitionSettings { get; set; }
        public AccountManagerAssignmentDefinition AccountManagerAssignmentDefinition { get; set; }
    }
}
