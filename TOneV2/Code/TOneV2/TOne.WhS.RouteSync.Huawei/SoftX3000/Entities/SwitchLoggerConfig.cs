﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Entities
{
    public class SwitchLoggerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_HuaweiSwitchLogger";

        public string Editor { get; set; }
    }
}
