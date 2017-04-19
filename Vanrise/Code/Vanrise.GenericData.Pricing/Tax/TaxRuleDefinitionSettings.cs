using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Pricing
{
    class TaxRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("14950E0B-749A-44D6-9464-F8B1A41D1EDF");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }
    }
}
