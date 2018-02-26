using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class ANumberSupplierCode : ICode, Vanrise.Entities.IDateEffectiveSettings
    {
        public long ANumberSupplierCodeId { get; set; }

        public int ANumberGroupId { get; set; }

        public int SupplierId { get; set; }

        public string Code { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
