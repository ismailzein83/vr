﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Retail.BusinessEntity.MainActionBPs.Entities
{
    public class RegularActionBPSettings : ActionBPSettings
    {
        public ActionProvisioner ActionProvisioner { get; set; }
    }
}
