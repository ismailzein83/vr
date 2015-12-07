using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierCode : ICode
    {
        public long SupplierCodeId { get; set; }

        public string Code { get; set; }

        public long ZoneId { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
