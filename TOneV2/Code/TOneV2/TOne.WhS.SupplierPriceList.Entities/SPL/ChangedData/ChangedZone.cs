using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ChangedZone : IChangedEntity
    {
        public long EntityId { get; set; }

        public DateTime EED { get; set; }
        public bool IsExcluded { get; set; }
    }
}
