using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZoneServiceQuery
    {
       
        public IEnumerable<long> ZoneIds { get; set; }
        public IEnumerable<int> ServiceIds { get; set; }
        public int SupplierId { get; set; }
        public DateTime EffectiveOn { get; set; }

    }
}
