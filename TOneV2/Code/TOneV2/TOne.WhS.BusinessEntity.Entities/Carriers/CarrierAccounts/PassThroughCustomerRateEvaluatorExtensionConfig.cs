using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PassThroughCustomerRateEvaluatorExtensionConfig : Vanrise.Entities.ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "WhS_BE_PassThroughCustomerRateEvaluator";

        public string Editor { get; set; }
    }
}
