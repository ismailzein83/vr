using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum CodeOnEachRowBEFieldType
    {
        Zone = 0,
        Code = 1,
        CodeBED = 2,
        CodeEED = 3,
        Rate = 4,
        RateBED = 5,
        RateEED = 6,
        Services = 7,
        RateChangeType = 8,
        Currency = 9
    }

    public class CodeOnEachRowBEFieldMappedValue : CodeOnEachRowMappedValue
    {
        public override Guid ConfigId
        {
            get { return new Guid("930FB07F-E599-4324-AC59-BAAB800ADC0E"); }
        }
        public CodeOnEachRowBEFieldType BEField { get; set; }

        public override void Execute(ICodeOnEachRowMappedValueContext context)
        {
            switch (BEField)
            {
                case CodeOnEachRowBEFieldType.Zone:
                    context.Value = context.Zone;
                    break;
                case CodeOnEachRowBEFieldType.Code:
                    context.Value = context.Code;
                    break;
                case CodeOnEachRowBEFieldType.CodeBED:
                    context.Value = context.CodeBED;
                    break;
                case CodeOnEachRowBEFieldType.CodeEED:
                    context.Value = context.CodeEED;
                    break;
                case CodeOnEachRowBEFieldType.Rate:
                    context.Value = context.Rate;
                    break;
                case CodeOnEachRowBEFieldType.RateBED:
                    context.Value = context.RateBED;
                    break;
                case CodeOnEachRowBEFieldType.RateEED:
                    context.Value = context.RateEED;
                    break;
                case CodeOnEachRowBEFieldType.Services:
                    ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
                    var services = zoneServiceConfigManager.GetZoneServicesNames(context.ServicesIds.ToList());
                    string servicesSymbol = string.Join(",", services);
                    context.Value = servicesSymbol;
                    break;
                case CodeOnEachRowBEFieldType.RateChangeType:
                    context.Value = GetRateChange(context.RateChangeType);
                    break;
                case CodeOnEachRowBEFieldType.Currency:
                    if (context.CurrencyId.HasValue)
                    {
                        CurrencyManager currencyManager = new CurrencyManager();
                        context.Value = currencyManager.GetCurrencySymbol(context.CurrencyId.Value);
                    }
                    break;
            }
        }
        private string GetRateChange(RateChangeType rateChangeType)
        {
            switch (rateChangeType)
            {
                case RateChangeType.Decrease:
                    return "D";
                case RateChangeType.Increase:
                    return "I";
                case RateChangeType.New:
                    return "N";
                case RateChangeType.NotChanged:
                    return "S";
            }
            return "S";
        }

    }
}
