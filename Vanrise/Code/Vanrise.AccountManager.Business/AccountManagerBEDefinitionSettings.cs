using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountManager.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.AccountManager.Business
{
    public class AccountManagerBEDefinitionSettings : BusinessEntityDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public List<AccountManagerAssignmentDefinition> AssignmentDefinitions { get; set; }

        public List<AccountManagerSubViewDefinition> SubViews { get; set; }
    }
}
