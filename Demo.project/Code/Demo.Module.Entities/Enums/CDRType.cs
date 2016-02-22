using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{

    public enum CDRType
    {

        [Description("Interconnect")]
        Interconnect = 1,

        [Description("International")]
        International = 2
    }

}
