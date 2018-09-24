using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.MainExtensions
{
    public enum CodesByZoneBEFieldType
    {
        Zone = 0,
        Codes = 1,
        EffectiveDate = 2,
        Rate = 3,
        RateBED = 4,
        RateEED = 5,
        Services = 6,
        RateChangeType = 7,
        Currency = 8,
        Increment = 9,
        CodeGroup = 10,
        Country = 11,
        ZoneNameWithoutCountryName = 12
    }

    public class CodesByZoneBEFieldMappedValue : CodesByZoneMappedValue
    {
        public override Guid ConfigId
        {
            get { return new Guid("40157184-07BF-4539-8160-DB54DF844F05"); }
        }
        public CodesByZoneBEFieldType BEField { get; set; }

        public override void Execute(ICodesByZoneMappedValueContext context)
        {
            var saleZoneManager = new SaleZoneManager();
            switch (BEField)
            {
                case CodesByZoneBEFieldType.Zone:
                    context.Value = context.ZoneNotification.ZoneName;
                    break;
                case CodesByZoneBEFieldType.Codes:
                    context.Value = GetCodesValue(context);
                    break;
                case CodesByZoneBEFieldType.EffectiveDate:
                    context.Value = GetEffectiveDate(context.ZoneNotification.Codes, context.ZoneNotification.Rate);
                    break;
                case CodesByZoneBEFieldType.Rate:
                    if (context.ZoneNotification.Rate != null)
                        context.Value = context.ZoneNotification.Rate.Rate;
                    break;
                case CodesByZoneBEFieldType.RateBED:
                    if (context.ZoneNotification.Rate != null)
                        context.Value = context.ZoneNotification.Rate.BED;
                    break;
                case CodesByZoneBEFieldType.RateEED:
                    if (context.ZoneNotification.Rate != null)
                        context.Value = context.ZoneNotification.Rate.EED;
                    break;
                case CodesByZoneBEFieldType.Services:
                    if (context.ZoneNotification.Rate != null && context.ZoneNotification.Rate.ServicesIds != null)
                    {
                        ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
                        var services = zoneServiceConfigManager.GetZoneServicesNames(context.ZoneNotification.Rate.ServicesIds.ToList());
                        string servicesSymbol = string.Join(",", services);
                        context.Value = servicesSymbol;
                    }
                    break;
                case CodesByZoneBEFieldType.RateChangeType:
                    if (context.ZoneNotification.Rate != null)
                        context.Value = GetRateChange(context.ZoneNotification.Rate.RateChangeType, context.CustomerId);
                    break;
                case CodesByZoneBEFieldType.Currency:
                    if (context.ZoneNotification.Rate != null && context.ZoneNotification.Rate.CurrencyId.HasValue)
                    {
                        var currencyManager = new CurrencyManager();
                        context.Value = currencyManager.GetCurrencySymbol(context.ZoneNotification.Rate.CurrencyId.Value);
                    }
                    break;
                case CodesByZoneBEFieldType.Increment:
                    context.Value = context.ZoneNotification.Increment;
                    break;
                case CodesByZoneBEFieldType.CodeGroup:
                    context.Value = new SalePriceListManager().GetCodeGroupFromCodes(context.ZoneNotification.Codes).Code;
                    break;
                case CodesByZoneBEFieldType.Country:
                    if (context.ZoneNotification.ZoneId.HasValue)
                        context.Value = saleZoneManager.GetCountryNameByZoneId(context.ZoneNotification.ZoneId.Value);
                    break;
                case CodesByZoneBEFieldType.ZoneNameWithoutCountryName:
                    if (context.ZoneNotification.ZoneId.HasValue)
                        context.Value = saleZoneManager.GetZoneNameWithoutCountryName(context.ZoneNotification.ZoneId, context.ZoneNotification.ZoneName);
                    break;
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
        private string GetCodesValue(ICodesByZoneMappedValueContext context)
        {
            List<string> codes = new List<string>();

            var codeNotifactions = context.ZoneNotification.Codes;

            var allCodesHasEED = codeNotifactions.All(c => c.EED.HasValue);
            if (allCodesHasEED) codes.AddRange(codeNotifactions.Select(c => c.Code));
            else
            {
                foreach (SalePLCodeNotification codeNotification in codeNotifactions)
                {
                    if (!codeNotification.EED.HasValue)
                        codes.Add(codeNotification.Code);
                }
            }
            return string.Join(context.Delimiter.ToString(), codes);
        }

        private DateTime GetEffectiveDate(IEnumerable<SalePLCodeNotification> codesNotificatons, SalePLRateNotification rate)
        {
            DateTime maxCodeBED = codesNotificatons.Select(item => item.BED).Max();
            return maxCodeBED > rate.BED ? maxCodeBED : rate.BED;
        }

    }
}
