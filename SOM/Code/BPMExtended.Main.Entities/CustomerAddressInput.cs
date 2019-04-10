using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class CustomerAddressInput
    {
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressSeq { get; set; }
        public string CustomerId { get; set; }
        public string ContactId { get; set; }
        public string AccountId { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }

    }


    public class CustomerAddressOutput
    {

    }
}
