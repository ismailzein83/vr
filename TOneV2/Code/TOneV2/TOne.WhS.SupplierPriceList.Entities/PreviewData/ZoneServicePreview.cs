using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class ZoneServicePreview
    {
        public string ZoneName { get; set; }
        public List<int> SystemServiceIds { get; set; }
        public DateTime? SystemServicesBED { get; set; }
        public DateTime? SystemServicesEED { get; set; }
        public List<int> ImportedServiceIds { get; set; }
        public DateTime? ImportedServicesBED { get; set; }
        public ZoneServiceChangeType ZoneServicesChangeType { get; set; }

    }
}
