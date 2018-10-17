using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class DAProfCalcAlertRuleFilterConfig :  ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Analytic_DAProfCalcAlertRuleFilterConfig";

        public string Editor { get; set; }
    }
}