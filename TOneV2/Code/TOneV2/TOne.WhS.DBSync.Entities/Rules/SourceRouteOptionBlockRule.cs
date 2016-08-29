using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceRouteOptionBlockRule : SourceBaseRule
    {
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }
        public int? SupplierZoneId { get; set; }
    }
}
