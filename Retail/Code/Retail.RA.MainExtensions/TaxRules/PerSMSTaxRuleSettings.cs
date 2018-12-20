using System;
using System.Collections.Generic;
using System.Text;
using Retail.RA.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.MainExtensions
{
    public class PerSMSTaxRuleSettings : SMSTaxRuleSettings
    {
        public override Guid ConfigId { get { return new Guid("E9A50C64-A4D2-4704-BCC3-E48752AA69E6"); } }

        public decimal TaxPerSMS { get; set; }

        protected override void Execute(ISMSTaxRuleContext context)
        {
            context.TotalTaxValue = context.NumberOfSMSs * TaxPerSMS;
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            Dictionary<string, object> settingsValuesByName = new Dictionary<string, object>();
            settingsValuesByName.Add("Tax per SMS", TaxPerSMS);
            return settingsValuesByName;
        }

        public override string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Tax per SMS: {0}", TaxPerSMS));
            return description.ToString();
        }
    }
}
