using System;
using System.Linq;
using Vanrise.Common.Business;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

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
        Currency = 9,
        CodeChangeType = 10,
        Increment = 11,
        CodeRateChangeType = 12,
        EffectiveDate = 13,
        CodeGroup = 14,
        Country = 15,
        ZoneNameWithoutCountryName = 16,
        AreaCode =17
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
            var saleZoneManager = new SaleZoneManager();
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
                    context.Value = GetRateChange(context.RateChangeType, context.CustomerId);
                    break;
                case CodeOnEachRowBEFieldType.CodeChangeType:
                    context.Value = GetCodeChange(context.CodeChangeType, context.CustomerId);
                    break;
                case CodeOnEachRowBEFieldType.Currency:
                    if (context.CurrencyId.HasValue)
                    {
                        CurrencyManager currencyManager = new CurrencyManager();
                        context.Value = currencyManager.GetCurrencySymbol(context.CurrencyId.Value);
                    }
                    break;
                case CodeOnEachRowBEFieldType.Increment:
                    context.Value = context.Increment;
                    break;
                case CodeOnEachRowBEFieldType.CodeRateChangeType:
                    context.Value = GetCodeRateChange(context.CustomerId, context.RateChangeType, context.CodeChangeType);
                    break;
                case CodeOnEachRowBEFieldType.EffectiveDate:
                    context.Value = GetEffectiveDate(context.CodeBED, context.RateBED);
                    break;
                case CodeOnEachRowBEFieldType.CodeGroup:
                    context.Value = new CodeGroupManager().GetMatchCodeGroup(context.Code).Code;
                    break;
                case CodeOnEachRowBEFieldType.Country:
                    if(context.ZoneId.HasValue)
                        context.Value = saleZoneManager.GetCountryNameByZoneId(context.ZoneId.Value);
                    break;
                case CodeOnEachRowBEFieldType.ZoneNameWithoutCountryName:
                    if (context.ZoneId.HasValue)
                        context.Value = saleZoneManager.GetZoneNameWithoutCountryName(context.ZoneId, context.Zone);
                    break;
                case CodeOnEachRowBEFieldType.AreaCode:
                    context.Value = saleZoneManager.GetAreaCodeFromCode(context.Code);
                    break;
            }
        }
        private DateTime? GetEffectiveDate(DateTime? codeBED, DateTime? rateBED)
        {
            return codeBED > rateBED ? codeBED : rateBED;
        }
        private string GetCodeRateChange(int customerId, RateChangeType rateChangeType, CodeChange codeChangeType)
        {
            var codeChangeTypeDescriptions = new CarrierAccountManager().GetCustomerCodeChangeTypeSettings(customerId);

            switch (codeChangeType)
            {
                case CodeChange.New:
                    return codeChangeTypeDescriptions.NewCode;
                case CodeChange.Moved:
                    return codeChangeTypeDescriptions.NewCode;
                case CodeChange.Closed:
                    return codeChangeTypeDescriptions.ClosedCode;
                default: return GetRateChange(rateChangeType, customerId);
            }
        }
        private string GetRateChange(RateChangeType rateChangeType, int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var rateChangeTypeDescriptions = carrierAccountManager.GetCustomerRateChangeTypeSettings(customerId);
            switch (rateChangeType)
            {
                case RateChangeType.Decrease:
                    return rateChangeTypeDescriptions.DecreasedRate;
                case RateChangeType.Increase:
                    return rateChangeTypeDescriptions.IncreasedRate;
                case RateChangeType.New:
                    return rateChangeTypeDescriptions.NewRate;
                case RateChangeType.NotChanged:
                    return rateChangeTypeDescriptions.NotChanged;
                case RateChangeType.Deleted:
                    return rateChangeTypeDescriptions.DeletedRate;
            }
            return rateChangeTypeDescriptions.NotChanged;
        }

        private string GetCodeChange(CodeChange codeChangeType, int customerId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            var codeChangeTypeDescriptions = carrierAccountManager.GetCustomerCodeChangeTypeSettings(customerId);
            switch (codeChangeType)
            {
                case CodeChange.NotChanged:
                    return codeChangeTypeDescriptions.NotChangedCode;
                case CodeChange.New:
                    return codeChangeTypeDescriptions.NewCode;
                case CodeChange.Moved:
                    return codeChangeTypeDescriptions.NewCode;
                case CodeChange.Closed:
                    return codeChangeTypeDescriptions.ClosedCode;
            }
            return codeChangeTypeDescriptions.NotChangedCode;
        }

    }
}
