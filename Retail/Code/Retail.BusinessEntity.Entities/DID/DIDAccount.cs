using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class DIDAccount
    {
        public long DIDAccountId { get; set; }

        public DID DID { get; set; }

        public Account Account { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
