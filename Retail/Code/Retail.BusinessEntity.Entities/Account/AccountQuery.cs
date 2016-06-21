using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountQuery
    {
        public string Name { get; set; }

        public IEnumerable<int> AccountTypeIds { get; set; }

        public long? ParentAccountId { get; set; }
    }
}
