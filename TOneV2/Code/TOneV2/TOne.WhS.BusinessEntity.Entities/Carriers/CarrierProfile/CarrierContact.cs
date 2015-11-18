using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CarrierContactType
    {
        BillingContactPerson = 1
    }
    public class CarrierContact
    {
        public CarrierContactType Type { get; set; }

        public string Description { get; set; }
    }
}
