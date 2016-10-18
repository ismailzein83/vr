using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class TariffRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("b2061c48-a2c9-4494-a707-0e84a195b5e5"); }
        }
    }
}
