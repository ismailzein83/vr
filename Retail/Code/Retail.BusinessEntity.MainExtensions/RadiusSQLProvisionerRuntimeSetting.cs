﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.MainExtensions
{
    public class RadiusSQLProvisionerRuntimeSetting : ActionProvisioner
    {
        public override void Execute(IActionProvisioningContext context)
        {
            context.ExecutionOutput = new ActionProvisioningOutput
            {
                Result = ActionProvisioningResult.Succeeded
            };
        }
    }
}
