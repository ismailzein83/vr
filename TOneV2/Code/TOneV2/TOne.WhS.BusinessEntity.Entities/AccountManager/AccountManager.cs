using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class AccountManager
    {
        public int? CarrierAccountId { get; set; }
        public int UserId { get; set; }
        public CarrierAccountType RelationType { get; set; }
    }
}
