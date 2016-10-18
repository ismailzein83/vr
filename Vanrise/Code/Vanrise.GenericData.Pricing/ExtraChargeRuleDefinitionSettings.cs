using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class ExtraChargeRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("7fd67c90-3ff6-4e5d-9052-bb0a87ab371e"); }
        }
    }
}
