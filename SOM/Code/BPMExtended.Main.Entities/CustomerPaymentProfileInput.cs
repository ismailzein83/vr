using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerPaymentProfileInput
    {
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string PaymentMethodId { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }
    }


}
