using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class PricingTemplateEditorRuntime
    {
        public PricingTemplate Entity { get; set; }

        public PricingTemplateRulesEditorRuntime RulesEditorRuntime { get; set; }
    }

    public class PricingTemplateRulesEditorRuntime
    {
        public Dictionary<int, string> CountryNameByIds { get; set; }

        public Dictionary<long, string> ZoneNameByIds { get; set; }
    }
}
