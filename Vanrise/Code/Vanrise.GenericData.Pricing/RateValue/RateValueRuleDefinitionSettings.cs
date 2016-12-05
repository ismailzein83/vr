using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("fc76233f-5f8f-4b5e-bf10-1e77ea24fd35");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }
    }
}
