﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainActionBPs.Entities
{
    public class RegularActionBPDefinitionSettings : ActionBPDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("b2255268-649b-4648-8584-c00ab98e56de"); } }
        public override string RuntimeEditor { get { return "retail-be-actionbpdefinition-runtimesettings-regular"; } }

        public AccountProvisionerDefinitionSettings ProvisionerDefinitionSettings { get; set; }
        public AccountProvisionDefinitionPostAction ProvisionDefinitionPostAction { get; set; }

    }
}
