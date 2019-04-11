using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerCreationInput
    {
        public string CustomerCategoryId { get; set; }
        public string PaymentMethodId { get; set; }
        public string RatePlanId { get; set; }
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerId { get; set; }
        public string CSO { get; set; }
        public string BankCode { get; set; }
        public string AccountNumber { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }
    }
}
