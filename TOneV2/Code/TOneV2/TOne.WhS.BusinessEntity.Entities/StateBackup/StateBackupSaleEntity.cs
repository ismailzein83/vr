using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupCustomer : StateBackupType
    {
        public int OwnerId { get; set; }
        public SalePriceListOwnerType OwnerType { get; set; }
    }
}
