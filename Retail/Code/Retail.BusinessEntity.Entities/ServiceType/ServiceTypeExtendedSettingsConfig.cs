﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Entities
{
    public class ServiceTypeExtendedSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_BE_ServiceTypeExtendedSettingsConfig";

        public string Editor { get; set; }
    }
}
