using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class CustomerPaymentProfileInput
    {
        public string AccountNumber { get; set; }
        public string AccountOwner { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string DebitCreditCard { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }
    }


}
