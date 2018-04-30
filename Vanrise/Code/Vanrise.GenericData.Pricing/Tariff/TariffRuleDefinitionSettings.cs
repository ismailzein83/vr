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
        public static Guid CONFIG_ID = new Guid("b2061c48-a2c9-4494-a707-0e84a195b5e5");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }
        public override List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add("Call Fee");
            fieldNames.Add("First Period");
            fieldNames.Add("First Period Rate");
            fieldNames.Add("Fraction Unit");
            fieldNames.Add("Pricing Unit");
            return fieldNames;
        }
    }
}
