using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Rules.Pricing.MainExtensions.RateValue;
using Vanrise.Common;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueRuleDefinitionSettings : GenericRuleDefinitionSettings
    {
        public static Guid CONFIG_ID = new Guid("fc76233f-5f8f-4b5e-bf10-1e77ea24fd35");

        public override Guid ConfigId
        {
            get { return CONFIG_ID; }
        }

        public override bool SupportUpload
        {
            get
            {
                return true;
            }
        }

        public override List<string> GetFieldNames()
        {
            List<string> fieldNames = new List<string>();
            fieldNames.Add("Currency");
            fieldNames.Add("Normal Rate");
            RateTypeManager manager = new RateTypeManager();
            IEnumerable<RateTypeInfo> rateTypes = manager.GetAllRateTypes();
            if (rateTypes != null)
            {
                foreach (var rateType in rateTypes)
                    fieldNames.Add(rateType.Name);
            }
            return fieldNames;
        }

        public override void CreateGenericRuleFromExcel(ICreateGenericRuleFromExcelContext context)
        {
            Dictionary<string, object> parsedFields = context.ParsedGenericRulesFields;
            RateTypeManager rateTypeManager = new RateTypeManager();
            IEnumerable<RateTypeInfo> rateTypes = rateTypeManager.GetAllRateTypes();

            if (parsedFields != null && parsedFields.Count!=0)
            {
                RateValueRule rateRule = new RateValueRule();
                FixedRateValueSettings fixedRateValueSettings = new FixedRateValueSettings();
                Dictionary<int, decimal> ratesByRateType = null;
                var currencyField = parsedFields.GetRecord("Currency");
                var normalRateField = parsedFields.GetRecord("Normal Rate");

                if (currencyField == null)
                {
                    context.ErrorMessage = "The currency field is required.";
                    return;
                }
                else if(currencyField!=null)
                {
                    CurrencyManager currencyManager = new CurrencyManager();
                    Currency currency = currencyManager.GetCurrencyBySymbol(currencyField.ToString().Trim().ToLower());
                    if (currency != null)
                    {
                        fixedRateValueSettings.CurrencyId = currency.CurrencyId;
                    }
                    else
                    {
                        context.ErrorMessage = string.Format("The currency symbol {0} does not exist.", currencyField.ToString());
                        return;
                    }

                }
                if (normalRateField == null)
                {
                    context.ErrorMessage = "The normal rate field is required.";
                    return;
                }
                else if(normalRateField!=null)
                {
                    if (normalRateField is Decimal)
                    {
                        fixedRateValueSettings.NormalRate = (Decimal)normalRateField;
                    }
                    else
                    {
                        Decimal result;
                        if (Decimal.TryParse(normalRateField.ToString(), out result))
                        {
                            fixedRateValueSettings.NormalRate = result;
                        }
                        else
                        {
                            context.ErrorMessage = "The normal rate should be a number.";
                            return;
                        }
                    }
                }

                foreach (var rateType in rateTypes)
                {
                    var rateTypeField = parsedFields.GetRecord(rateType.Name);
                    if (rateTypeField == null)
                    {
                        continue;
                    }
                    else if (rateTypeField is Decimal)
                    {
                        if (ratesByRateType == null)
                            ratesByRateType = new Dictionary<int, decimal>();

                        ratesByRateType.Add(rateType.RateTypeId, (Decimal)rateTypeField);
                    }
                    else
                    {
                        Decimal result;
                        if (Decimal.TryParse(rateTypeField.ToString(), out result))
                        {
                            if (ratesByRateType == null)
                                ratesByRateType = new Dictionary<int, decimal>();

                            ratesByRateType.Add(rateType.RateTypeId, result);
                        }
                        else
                        {
                            context.ErrorMessage = string.Format("The rate type {0} should be a number.", rateType.Name.ToString());
                            break;
                        }
                    }
                }
                fixedRateValueSettings.RatesByRateType = ratesByRateType;
                rateRule.Settings = fixedRateValueSettings;
                context.GenericRule = rateRule;

            }
        }
    }
}
