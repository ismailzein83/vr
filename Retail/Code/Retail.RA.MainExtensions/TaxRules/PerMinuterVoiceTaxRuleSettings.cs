using System;
using System.Collections.Generic;
using System.Text;
using Retail.RA.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.MainExtensions
{
    public class PerMinuterVoiceTaxRuleSettingsA : VoiceTaxRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("FC96DCD0-F1F4-45A0-88E4-569A6BC0FA13"); } }

        public decimal TaxPerMinute { get; set; }

        protected override void Execute(IVoiceTaxRuleContext context)
        {
            context.TotalTaxValue = (context.DurationInSeconds / 60) * TaxPerMinute;
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            Dictionary<string, object> settingsValuesByName = new Dictionary<string, object>();
            settingsValuesByName.Add("Tax per minute", TaxPerMinute);
            return settingsValuesByName;
        }

        public override string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Tax per minute: {0}", TaxPerMinute));
            return description.ToString();
        }
    }
}