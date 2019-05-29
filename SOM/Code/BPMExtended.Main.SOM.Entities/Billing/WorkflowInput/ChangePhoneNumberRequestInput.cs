using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ChangePhoneNumberRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string OldDirectoryNumber { get; set; }
        public string NewDirectoryNumber { get; set; }
        public string OldRatePlanId { get; set; }
        public string NewRatePlanId { get; set; }
        public List<ServiceData> FeesToRemove { get; set; }
    }

}
    