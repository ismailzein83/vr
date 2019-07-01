using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class FAFManagementRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string PhoneNumber { get; set; }
        public List<FAFNumber> FAFNumbers { get; set; }
        public string FAFGroupId { get; set; }

    }
}
