using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSTN.BusinessEntity.Entities
{
    public enum SwitchTrunkType
    {
        Local = 0,
        Transit = 1,
        International = 2,
        Mobile = 3,
        Wireless = 4
    }

    public enum SwitchTrunkDirection {
        In = 0,
        Out = 1,
        InOut = 2,
        Unknown = 3
    }
}
