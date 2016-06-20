using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountType2
    {
        public int AccountTypeId { get; set; }

        public string Name { get; set; }

        public AccountTypeSettings Settings { get; set; }
    }

    public class AccountTypeSettings
    {
        public string Title { get; set; }

        public bool CanBeRootAccount { get; set; }

        public List<int> SupportedParentAccountTypeIds { get; set; }

        public List<AccountPartDefinition> PartDefinitions { get; set; }
    }

}
