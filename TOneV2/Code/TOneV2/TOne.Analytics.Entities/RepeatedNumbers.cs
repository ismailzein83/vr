using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
   public  class RepeatedNumbers
    {
       public Byte SwitchID { get; set; }
        public int OurZoneID { get; set; }
        public string CustomerID { get; set; }
        public string SupplierID { get; set; }
        public string PhoneNumber { get; set; }
        public int Attempts { get; set; }
        public Decimal DurationsInMinutes { get; set; }
       

    }
}
