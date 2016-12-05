using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("5969790e-1bd4-45e4-be39-b8d7fa6a1842");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }
    }
}
