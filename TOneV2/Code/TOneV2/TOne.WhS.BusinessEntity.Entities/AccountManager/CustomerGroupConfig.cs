using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class CustomerGroupConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_BE_CustomerGroup";
        public string Editor { get; set; }
        public string BehaviorFQTN { get; set; }
    }
}
