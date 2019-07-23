using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Retail.RA.Business
{
    public class PostpaidTaxRuleSettings
    {
        public UsagePostpaidTaxRuleSettings UsageSettings { get; set; }
        public NonUsagePostpaidTaxRuleSettings NonUsageSettings { get; set; }
        public string GetDescription(IGenericRuleSettingsDescriptionContext context)
        {
            StringBuilder description = new StringBuilder();
            if (UsageSettings != null)
            {
                if (UsageSettings.OverallTaxPercentage.HasValue)
                    description.Append(string.Format("Overall Postpaid Usage Tax Percentage: {0}.", UsageSettings.OverallTaxPercentage.Value));

                if (UsageSettings.VoiceSettings != null && UsageSettings.VoiceSettings.ChargeType.HasValue)
                {
                    if (UsageSettings.VoiceSettings.ChargeType.Value == PostpaidVoiceTaxChargeType.Overall && UsageSettings.VoiceSettings.TaxPercentage.HasValue)
                    {
                        description.Append(string.Format("Overall Postpaid Voice Tax Percentage: {0}.", UsageSettings.VoiceSettings.TaxPercentage.Value));
                    }
                    else if (UsageSettings.VoiceSettings.ChargeType.Value == PostpaidVoiceTaxChargeType.PerMinute && UsageSettings.VoiceSettings.TaxPerMinute.HasValue)
                    {
                        if (description.Length > 0)
                            description.Append(" ");
                        description.Append(string.Format("Postpaid Voice Tax Per Minute: {0}.", UsageSettings.VoiceSettings.TaxPerMinute.Value));
                    }
                }
                if (UsageSettings.SMSSettings != null && UsageSettings.SMSSettings.ChargeType.HasValue)
                {
                    if (UsageSettings.SMSSettings.ChargeType.Value == PostpaidSMSTaxChargeType.Overall && UsageSettings.SMSSettings.TaxPercentage.HasValue)
                    {
                        if (description.Length > 0)
                            description.Append(" ");
                        description.Append(string.Format("Overall Postpaid SMS Tax Percentage: {0}.", UsageSettings.SMSSettings.TaxPercentage.Value));
                    }
                    else if (UsageSettings.SMSSettings.ChargeType.Value == PostpaidSMSTaxChargeType.PerSMS && UsageSettings.SMSSettings.TaxPerSMS.HasValue)
                    {
                        if (description.Length > 0)
                            description.Append(" ");
                        description.Append(string.Format("Postpaid SMS Tax Per SMS: {0}.", UsageSettings.SMSSettings.TaxPerSMS.Value));
                    }
                }
            }

            if (NonUsageSettings != null)
            {
                if (NonUsageSettings.TransactionSettings != null)
                {
                    if (NonUsageSettings.TransactionSettings.TaxPercentage.HasValue)
                    {
                        if (description.Length > 0)
                            description.Append(" ");
                        description.Append(string.Format("Overall Postpaid Non Usage Tax Percentage: {0}.", NonUsageSettings.TransactionSettings.TaxPercentage.Value));
                    }
                }
            }

            return description.ToString();
        }

        public void ApplyPostpaidTaxRule(IPostpaidTaxRuleContext context)
        {
            if (context == null)
                return;

            if (UsageSettings != null)
            {
                if (UsageSettings.VoiceSettings != null && context.VoiceContext != null)
                {
                    if(UsageSettings.UsagePostpaidTaxRuleType == UsageTaxRuleType.OverallUsage && UsageSettings.OverallTaxPercentage.HasValue && context.VoiceContext.TotalAmount.HasValue)
                        context.VoiceContext.TotalTaxValue = context.VoiceContext.TotalAmount.Value * UsageSettings.OverallTaxPercentage.Value / 100;

                   else if(UsageSettings.UsagePostpaidTaxRuleType == UsageTaxRuleType.PerTrafficType)
                   {
                        if (UsageSettings.VoiceSettings.ChargeType == PostpaidVoiceTaxChargeType.Overall && context.VoiceContext.TotalAmount.HasValue && UsageSettings.VoiceSettings.TaxPercentage.HasValue)
                            context.VoiceContext.TotalTaxValue = context.VoiceContext.TotalAmount.Value * UsageSettings.VoiceSettings.TaxPercentage.Value / 100;

                        else if (UsageSettings.VoiceSettings.ChargeType == PostpaidVoiceTaxChargeType.PerMinute && context.VoiceContext.DurationInSeconds.HasValue && UsageSettings.VoiceSettings.TaxPerMinute.HasValue )
                            context.VoiceContext.TotalTaxValue = (context.VoiceContext.DurationInSeconds.Value / 60) * UsageSettings.VoiceSettings.TaxPerMinute.Value;
                    }
                }
                if (UsageSettings.SMSSettings != null && context.SMSContext != null)
                {
                    if (context.SMSContext.TotalAmount.HasValue)
                    {
                        if (UsageSettings.UsagePostpaidTaxRuleType == UsageTaxRuleType.OverallUsage && UsageSettings.OverallTaxPercentage.HasValue && context.SMSContext.TotalAmount.HasValue)
                            context.SMSContext.TotalTaxValue = context.SMSContext.TotalAmount.Value * UsageSettings.OverallTaxPercentage.Value / 100;

                        else if(UsageSettings.UsagePostpaidTaxRuleType == UsageTaxRuleType.PerTrafficType)
                        {
                            if (UsageSettings.SMSSettings.ChargeType == PostpaidSMSTaxChargeType.Overall && context.SMSContext.TotalAmount.HasValue && UsageSettings.SMSSettings.TaxPercentage.HasValue)
                                context.SMSContext.TotalTaxValue = context.SMSContext.TotalAmount.Value * UsageSettings.SMSSettings.TaxPercentage.Value / 100;

                            else if (UsageSettings.SMSSettings.ChargeType == PostpaidSMSTaxChargeType.PerSMS && context.SMSContext.NumberOfSMS.HasValue && UsageSettings.SMSSettings.TaxPerSMS.HasValue)
                                context.SMSContext.TotalTaxValue = context.SMSContext.NumberOfSMS.Value * UsageSettings.SMSSettings.TaxPerSMS.Value;
                        }
                      
                    }
                }
            }

            if (NonUsageSettings != null)
            {
                if (NonUsageSettings.TransactionSettings != null && context.TransactionContext != null)
                {
                    if (NonUsageSettings.TransactionSettings.TaxPercentage.HasValue && context.TransactionContext.TotalAmount.HasValue)
                        context.TransactionContext.TotalTaxValue = context.TransactionContext.TotalAmount.Value * NonUsageSettings.TransactionSettings.TaxPercentage.Value / 100;
                }
            }
        }
    }

    public enum UsageTaxRuleType { OverallUsage = 0, PerTrafficType = 1 }
    public class UsagePostpaidTaxRuleSettings
    {
        public UsageTaxRuleType? UsagePostpaidTaxRuleType { get; set; }
        public decimal? OverallTaxPercentage { get; set; }
        public PostpaidVoiceTaxRuleSettings VoiceSettings { get; set; }
        public PostpaidSMSTaxRuleSettings SMSSettings { get; set; }
    }

    public class NonUsagePostpaidTaxRuleSettings
    {
        public PostpaidTransactionTaxRuleSettings TransactionSettings { get; set; }
    }

    public enum PostpaidVoiceTaxChargeType { Overall = 0, PerMinute = 1 }
    public class PostpaidVoiceTaxRuleSettings
    {
        public PostpaidVoiceTaxChargeType? ChargeType { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxPerMinute { get; set; }
    }

    public enum PostpaidSMSTaxChargeType { Overall = 0, PerSMS = 1 }
    public class PostpaidSMSTaxRuleSettings
    {
        public PostpaidSMSTaxChargeType? ChargeType { get; set; }
        public decimal? TaxPercentage { get; set; }
        public decimal? TaxPerSMS { get; set; }
    }

    public class PostpaidTransactionTaxRuleSettings
    {
        public decimal? TaxPercentage { get; set; }
    }
}

