using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Analytic.Entities
{
    public class MeasureStyleRuleTemplate : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "Analytic_MeasureStyleRuleTemplates";

        public string Editor { get; set; }
    }
}
