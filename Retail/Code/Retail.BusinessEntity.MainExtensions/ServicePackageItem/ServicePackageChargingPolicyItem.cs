﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ServicePackageItem
{
    public class ServicePackageChargingPolicyItem : Entities.PackageItemSettings
    {
        public ChargingPolicySettings ChargingPolicySettings { get; set; }
    }
}
