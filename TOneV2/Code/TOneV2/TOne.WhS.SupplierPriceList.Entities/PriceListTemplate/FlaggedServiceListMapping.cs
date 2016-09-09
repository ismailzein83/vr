using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.ExcelConversion.Entities;

namespace TOne.WhS.SupplierPriceList.Entities
{
    public class FlaggedServiceListMapping
    {
        public int ZoneServiceConfigId { get; set; }
        public ListMapping ServiceListMapping { get; set; }
    }
}
