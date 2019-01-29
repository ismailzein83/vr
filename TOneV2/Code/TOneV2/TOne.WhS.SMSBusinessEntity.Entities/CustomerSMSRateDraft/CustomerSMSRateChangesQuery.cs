using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class CustomerSMSRateChangesQuery
    {
        public int CustomerID { get; set; }

        public char? CountryChar { get; set; }
    }
}
