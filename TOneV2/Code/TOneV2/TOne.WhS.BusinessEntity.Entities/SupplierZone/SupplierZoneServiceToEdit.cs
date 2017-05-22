using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
   public class SupplierZoneServiceToEdit
    {
      public long SupplierZoneServiceId { get; set; }
      public string ZoneName {get;set;}
      public int SupplierId {get;set;}
      public DateTime BED { get; set; }
      public List<ZoneService> Services { get; set; }
    }
}
