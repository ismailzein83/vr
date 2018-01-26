using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupSaleEntity : StateBackupType
    {
        public override Guid ConfigId
        {
            get { return new Guid("1b590995-522c-42a9-835d-77b6957b6426"); }
        }

        public int OwnerId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }
        public IEnumerable<int> SellingProductCustomerIds { get; set; }

        public int? MasterOwnerId { get; set; }
    }
}
