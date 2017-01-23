using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class TQISuppplierInfo
    {
        public string SupplierName { get; set; }
        public string ZoneName { get; set; }
        public decimal Rate { get; set; }
        public List<ZoneService> ZoneServices { get; set; }
        public decimal NER { get; set; }
        public decimal ASR { get; set; }
        public decimal ACD { get; set; }
        public decimal TQI { get; set; }
        public decimal Duration { get; set; }

    }
}
