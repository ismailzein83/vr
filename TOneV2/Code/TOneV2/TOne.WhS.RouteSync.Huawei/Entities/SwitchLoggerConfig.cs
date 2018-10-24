﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.RouteSync.Huawei.Entities
{
    public class SwitchLoggerConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_RouteSync_HuaweiSynchronizerFTPLogger";

        public string Editor { get; set; }
    }
}
