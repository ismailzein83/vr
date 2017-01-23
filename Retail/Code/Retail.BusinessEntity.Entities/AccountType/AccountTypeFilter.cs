using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountTypeFilter
    {
        public Guid? AccountBEDefinitionId { get; set; }

        public long? ParentAccountId { get; set; }

        public bool RootAccountTypeOnly { get; set; }

        public bool IncludeHiddenAccountTypes { get; set; }

        public List<IAccountTypeFilter> Filters { get; set; }
    }
}
