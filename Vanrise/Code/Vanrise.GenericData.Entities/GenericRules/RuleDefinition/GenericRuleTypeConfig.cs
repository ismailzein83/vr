using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class GenericRuleTypeConfig:ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_GenericData_GenericRuleTypeConfig";

      //  public int GenericRuleTypeConfigId { get; set; }

        //public string Name { get; set; }

        //public string Title { get; set; }

        public string Editor { get; set; }

        public string RuntimeEditor { get; set; }

        public string RuleTypeFQTN { get; set; }

        public string RuleManagerFQTN { get; set; }

        public string FilterEditor { get; set; }
    }
}
