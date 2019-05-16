using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.Entities
{
    public class PabxContractDetail
    {
        public string Id { get; set; }

        public string PhoneNumber { get; set; }

        public Address Address { get; set; }

        public string DeviceId { get; set; }
    }
}
