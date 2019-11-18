using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.NIM.Entities
{
    public class ReservePhoneNumberInput
    {
        public long PhoneNumberId { get; set; }
    }

    public class ReservePhoneNumberOutput
    {
        public bool IsSucceeded { get; set; }
    }
}
