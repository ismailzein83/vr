using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class MarketPriceRouteOptionRule : RouteOptionRuleSettings
    {
        #region Properties
        public override Guid ConfigId { get { return new Guid("{9D66822E-ADB3-4088-8E94-33647E523C29}"); } }

        public int CurrencyId { get; set; }

        public Dictionary<string, MarketPrice> MarketPrices { get; set; }
        #endregion


        #region Public Methods
        public override void Execute(IRouteOptionRuleExecutionContext context, BaseRouteOptionRuleTarget target)
        {
            if (context.RouteRule != null && context.RouteRule.CorrespondentType == CorrespondentType.Override)
                return;

            MarketPrice tempMarketPrice;

            if (context.SaleZoneServiceIds == null)
                return;

            if (MarketPrices.TryGetValue(context.SaleZoneServiceIds, out tempMarketPrice))
            {
                if (target.SupplierRate < tempMarketPrice.ConvertedMinimum || target.SupplierRate > tempMarketPrice.ConvertedMaximum)
                    target.FilterOption = true;
            }
        }

        public override RouteOptionRuleSettingsEditorRuntime GetEditorRuntime()
        {
            MarketPriceEditorRuntime marketPriceEditorRuntime = new MarketPriceEditorRuntime();
            marketPriceEditorRuntime.Services = new Dictionary<int, string>();

            ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();

            foreach (MarketPrice itm in MarketPrices.Values)
            {
                foreach (int serviceId in itm.ServiceIds)
                {
                    if (!marketPriceEditorRuntime.Services.ContainsKey(serviceId))
                        marketPriceEditorRuntime.Services.Add(serviceId, zoneServiceConfigManager.GetServiceSymbol(serviceId));
                }
            }

            return marketPriceEditorRuntime;
        }

        public override void RefreshState(Vanrise.Rules.IRefreshRuleStateContext context)
        {
            if (MarketPrices == null)
                return;

            Vanrise.Common.Business.ConfigManager configManager = new Vanrise.Common.Business.ConfigManager();
            int systemCurrencyId = configManager.GetSystemCurrencyId();

            CurrencyExchangeRateManager currencyExchangeRateManager = new CurrencyExchangeRateManager();
            decimal exchangeRate = currencyExchangeRateManager.ConvertValueToCurrency(1, CurrencyId, systemCurrencyId, context.EffectiveDate);

            foreach (KeyValuePair<string, MarketPrice> marketPrice in MarketPrices)
            {
                MarketPrice currentItem = marketPrice.Value;
                currentItem.ConvertedMinimum = currentItem.Minimum * exchangeRate;
                currentItem.ConvertedMaximum = currentItem.Maximum * exchangeRate;
            }
        }

        public override RouteOptionRuleSettings BuildLinkedRouteOptionRuleSettings(ILinkedRouteOptionRuleContext context)
        {
            return new MarketPriceRouteOptionRule()
            {
                CurrencyId = this.CurrencyId,
                MarketPrices = Vanrise.Common.Utilities.CloneObject<Dictionary<string, MarketPrice>>(this.MarketPrices)
            };
        }
        #endregion
    }


    public class MarketPrice
    {
        public List<int> ServiceIds { get; set; }

        public decimal Minimum { get; set; }

        public decimal Maximum { get; set; }

        public decimal? ConvertedMinimum;

        public decimal? ConvertedMaximum;
    }

    public class MarketPriceEditorRuntime : RouteOptionRuleSettingsEditorRuntime
    {
        public Dictionary<int, string> Services { get; set; }
    }
}
