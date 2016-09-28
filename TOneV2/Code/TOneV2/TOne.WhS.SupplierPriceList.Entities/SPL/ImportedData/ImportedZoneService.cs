using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Entities.SPL
{
    public class ImportedZoneService : Vanrise.Entities.IDateEffectiveSettings
    {
        public int ServiceId { get; set; }
        public string ZoneName { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}
