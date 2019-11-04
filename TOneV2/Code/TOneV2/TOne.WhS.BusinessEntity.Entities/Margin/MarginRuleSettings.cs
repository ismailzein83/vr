using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class MarginRuleSettings
    {
        public int CurrencyId { get; set; }

        public List<MarginSettingItem> MarginSettingItems { get; set; }

        public void ApplyMarginRule(IApplyMarginRuleContext context)
        {
            MarginSettingItems.ThrowIfNull("MarginSettingItems");
            var currencyExchangeRateManager = new CurrencyExchangeRateManager();

            int marginCurrencyId = context.MarginCurrencyId.HasValue ? context.MarginCurrencyId.Value : new ConfigManager().GetSystemCurrencyId();
            DateTime effectiveDate = context.EffectiveDate.HasValue ? context.EffectiveDate.Value : DateTime.Now;
            var convertedMargin = currencyExchangeRateManager.ConvertValueToCurrency(context.Margin, marginCurrencyId, this.CurrencyId, effectiveDate);

            foreach (var marginSettingItem in MarginSettingItems)
            {
                if (!marginSettingItem.UpTo.HasValue || convertedMargin <= marginSettingItem.UpTo.Value)
                {
                    context.MarginCategory = marginSettingItem.Category;
                    return;
                }
            }
        }
    }

    public class MarginSettingItem
    {
        public decimal? UpTo { get; set; }

        public Guid Category { get; set; }
    }
}
