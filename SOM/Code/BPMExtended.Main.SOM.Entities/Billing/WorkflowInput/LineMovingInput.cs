using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class LineMovingInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string OldLinePathId { get; set; }
        public string NewLinePathId { get; set; }
        public bool SameSwitch { get; set; }
    }
}
