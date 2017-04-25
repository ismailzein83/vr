﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class ChargeableEntityDefinitionSettings : Vanrise.Entities.GenericLKUPDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7B651637-EEE9-4804-91E4-51ECC82D8DD0"); }
        }
        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-chargeableentitysettings";
            }
        }
    }
}
