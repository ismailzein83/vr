using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.MultiNet.Entities
{
    public enum MultiNetAccountType
    {
        [Description("Business Trunk")]
        BusinessTrunk = 0,
        [Description("POTS")]
        POTS = 1,
        [Description("IP Centrex")]
        IPCentrex = 2
    }

}
