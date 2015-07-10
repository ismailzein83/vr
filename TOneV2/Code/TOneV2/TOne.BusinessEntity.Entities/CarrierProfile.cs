using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.BusinessEntity.Entities
{
    public class CarrierProfile
    {
        public int ProfileID { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string BillingEmail { get; set; }
    }
}
