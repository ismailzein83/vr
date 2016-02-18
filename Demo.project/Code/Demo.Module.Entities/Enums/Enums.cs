using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public enum Direction {

        [Description("Outgoing")]
        Outgoing = 1,

        [Description("Incoming")]
        Incoming = 2
    }

    public enum Type
    {

        [Description("Interconnect")]
        Interconnect = 1,

        [Description("International")]
        International = 2
    }
   
}
