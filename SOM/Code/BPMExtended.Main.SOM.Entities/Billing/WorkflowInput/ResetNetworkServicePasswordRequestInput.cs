﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMExtended.Main.Entities;

namespace BPMExtended.Main.SOMAPI
{
    public class ResetNetworkServicePasswordRequestInput
    {
        public CommonInputArgument CommonInputArgument { get; set; }
        public string LinePathId { get; set; }
        public string ServiceId { get; set; }

    }
}
