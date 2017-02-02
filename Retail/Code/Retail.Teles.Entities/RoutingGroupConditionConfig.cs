using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Retail.Teles.Entities
{
    public class RoutingGroupConditionConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Retail_Teles_RoutingGroupCondition";
        public string Editor { get; set; }
    }
}
