using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public enum CDRDirection
    {

        [Description("Outgoing")]
        Outgoing = 1,

        [Description("Incoming")]
        Incoming = 2,

        [Description("Interconnect")]
        Interconnect = 3
    }
   
}
