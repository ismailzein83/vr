﻿using System;
using System.Collections.Generic;
using System.Text;
using Vanrise.Common.Business;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public abstract class SMSRateValueSettings
    {
        public abstract Guid ConfigId { get; }

        public int CurrencyId { get; set; }

        protected abstract void Execute(ISMSRateValueRuleContext context);

        public void ApplyRateValueRule(ISMSRateValueRuleContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);

        public virtual Dictionary<string, object> GetSettingsValuesByName()
        {
            return null;
        }
    }

    public class FixedSMSRateValueSettings : SMSRateValueSettings
    {
        public override Guid ConfigId { get { return new Guid("53f5fb2a-0390-4821-a795-9146615cf584"); } }
        public Decimal NormalRate { get; set; }
        public Dictionary<int, Decimal> RatesByRateType { get; set; }

        protected override void Execute(ISMSRateValueRuleContext context)
        {
            context.CurrencyId = this.CurrencyId;
            context.NormalRate = this.NormalRate;
            context.RatesByRateType = this.RatesByRateType;
        }

        public override string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Normal Rate: {0}", NormalRate));
            if (RatesByRateType != null)
            {
                RateTypeManager rateTypeManager = new RateTypeManager();

                description.Append("; Other Rates: ");
                foreach (KeyValuePair<int, Decimal> kvp in RatesByRateType)
                {
                    var rateType = rateTypeManager.GetRateType(kvp.Key);
                    string rateTypeName = (rateType != null) ? rateType.Name : kvp.Key.ToString();
                    description.Append(String.Format("{0}: {1}; ", rateTypeName, kvp.Value));
                }
            }
            return description.ToString();
        }

        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            Dictionary<string, object> ratesByNames = new Dictionary<string, object>();
            ratesByNames.Add("Normal Rate", NormalRate);
            if (RatesByRateType != null)
            {
                RateTypeManager rateTypeManager = new RateTypeManager();
                foreach (KeyValuePair<int, Decimal> kvp in RatesByRateType)
                {
                    var rateType = rateTypeManager.GetRateType(kvp.Key);
                    string rateTypeName = (rateType != null) ? rateType.Name : kvp.Key.ToString();
                    ratesByNames.Add(rateTypeName, kvp.Value);
                }
            }
            return ratesByNames;
        }
    }
}