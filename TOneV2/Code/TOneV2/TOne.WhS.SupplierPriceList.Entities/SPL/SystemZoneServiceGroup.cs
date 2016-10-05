using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class SystemZoneServiceGroup
    {
        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public bool HasChanged { get; set; }

        public List<int> ZoneServicesIds { get; set; }

    }
}
