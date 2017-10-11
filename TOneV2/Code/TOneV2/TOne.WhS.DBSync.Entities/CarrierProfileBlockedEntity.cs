using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
  public  class CarrierProfileBlockedStatusEntity
    {
        public bool IsCustomerBlocked { get; set; }
        public bool IsSupplierBlocked { get; set; }
        public string ProfileId { get; set; }
    }
}
