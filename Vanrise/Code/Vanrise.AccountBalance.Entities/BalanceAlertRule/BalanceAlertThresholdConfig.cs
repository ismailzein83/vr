﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceAlertThresholdConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_AccountBalance_BalanceAlertThreshold";
        public string Editor { get; set; }
    }
}
