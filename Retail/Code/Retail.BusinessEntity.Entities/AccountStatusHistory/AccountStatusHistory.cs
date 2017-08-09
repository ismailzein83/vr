using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountStatusHistory
    {
        public long AccountStatusHistoryId { get; set; }
        public long AccountId { get; set; }
        public Guid StatusId { get; set; }
        public DateTime StatusChangedDate { get; set; }
    }
}
