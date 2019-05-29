using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class CustomerAddressInput
    {
        public string City { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AddressSeq { get; set; }
        public CommonInputArgument CommonInputArgument { get; set; }

    }

}
