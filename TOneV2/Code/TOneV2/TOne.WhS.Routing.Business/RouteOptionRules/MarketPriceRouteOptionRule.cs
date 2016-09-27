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

        public List<MarketPrice> MarketPrices { get; set; }

        #endregion


        #region Public Methods
        public override void Execute(IRouteOptionRuleExecutionContext context, RouteOptionRuleTarget target)
        {
            throw new NotImplementedException();
        }

        public override RouteOptionRuleSettingsEditorRuntime GetEditorRuntime()
        {
            MarketPriceEditorRuntime marketPriceEditorRuntime = new MarketPriceEditorRuntime();
            marketPriceEditorRuntime.Services = new Dictionary<int, string>(); ;
            marketPriceEditorRuntime.Currency = new Dictionary<int, string>();

            ZoneServiceConfigManager zoneServiceConfigManager = new ZoneServiceConfigManager();
            CurrencyManager currencyManager = new CurrencyManager();

            foreach (MarketPrice itm in MarketPrices)
            {
                //Services
                foreach (int serviceId in itm.ServiceIds)
                {
                    if (!marketPriceEditorRuntime.Services.ContainsKey(serviceId))
                        marketPriceEditorRuntime.Services.Add(serviceId, zoneServiceConfigManager.GetServiceSymbol(serviceId));
                }

                //Currencies
                if (!marketPriceEditorRuntime.Currency.ContainsKey(itm.CurrencyId))
                    marketPriceEditorRuntime.Currency.Add(itm.CurrencyId, currencyManager.GetCurrencySymbol(itm.CurrencyId));
            }

            return marketPriceEditorRuntime;
        }


        #endregion
    }


    public class MarketPrice
    {
        public List<int> ServiceIds { get; set; }

        public int CurrencyId { get; set; }

        public decimal Minimum { get; set; }

        public decimal Maximum { get; set; }
    }

    public class MarketPriceEditorRuntime : RouteOptionRuleSettingsEditorRuntime
    {
        public Dictionary<int, string> Services { get; set; }

        public Dictionary<int, string> Currency { get; set; }
    }
}
