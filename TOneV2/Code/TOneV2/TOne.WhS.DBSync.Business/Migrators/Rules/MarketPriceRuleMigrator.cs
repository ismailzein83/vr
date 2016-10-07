using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups;
using TOne.WhS.Routing.Business.RouteRules.Percentages;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class MarketPriceRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Market Price"; }
        }

        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly Dictionary<string, ZoneServiceConfig> _allSaleEntityServiceFlags;
        CurrencySettingData _currencySettingData;
        readonly short[] _olsFlaggedServiceIds;
        readonly int _routeOptionRuleTypeId;
        public MarketPriceRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            _allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;

            var dtTableZoneServiceConfig = Context.MigrationContext.DBTables[DBTableName.ZoneServiceConfig];
            _allSaleEntityServiceFlags = (Dictionary<string, ZoneServiceConfig>)dtTableZoneServiceConfig.Records;

            SettingManager settingManager = new SettingManager();
            var _systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            _currencySettingData = (CurrencySettingData)_systemCurrencySetting.Data;

            _olsFlaggedServiceIds = _allSaleEntityServiceFlags.Select(f => short.Parse(f.Key)).ToArray();

            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            _routeOptionRuleTypeId = manager.GetRuleTypeId();
        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> routeRules = new List<SourceRule>();

            SourceMarketPriceDataManager dataManager = new SourceMarketPriceDataManager(Context.MigrationContext.ConnectionString);
            var rules = dataManager.GetSourceMarketPrices();
            var dicRules = GroupRulesDictionary(rules);
            foreach (var rule in dicRules.Values)
                routeRules.Add(GetSourceRuleFromZones(rule));
            return routeRules;
        }

        Dictionary<int, List<SourceMarketPrice>> GroupRulesDictionary(IEnumerable<SourceMarketPrice> rules)
        {
            Dictionary<int, List<SourceMarketPrice>> dicRules = new Dictionary<int, List<SourceMarketPrice>>();
            foreach (var routeRule in rules)
            {
                List<SourceMarketPrice> lstRules;
                if (!dicRules.TryGetValue(routeRule.SaleZoneID, out lstRules))
                {
                    lstRules = new List<SourceMarketPrice>();
                    dicRules.Add(routeRule.SaleZoneID, lstRules);
                }
                lstRules.Add(routeRule);
            }
            return dicRules;
        }
        SourceRule GetSourceRuleFromZones(IEnumerable<SourceMarketPrice> rules)
        {
            var settings = GetMarketPriceRuleSettings(rules);

            return new SourceRule
            {
                Rule = new Rule
                {
                    BED = DateTime.Now,
                    EED = null,
                    TypeId = _routeOptionRuleTypeId,
                    RuleDetails = Serializer.Serialize(settings)
                }
            };
        }

        RouteOptionRule GetMarketPriceRuleSettings(IEnumerable<SourceMarketPrice> rules)
        {
            RouteOptionRule settings = new RouteOptionRule()
            {
                BeginEffectiveTime = DateTime.Now,
                EndEffectiveTime = null,
                Description = "Market Price Rule",
                Name = "Market Price Rule",
                Settings = new MarketPriceRouteOptionRule
                {
                    MarketPrices = GetMarketPrices(rules),
                    CurrencyId = _currencySettingData.CurrencyId
                },
                Criteria = new RouteOptionRuleCriteria
                {
                    SaleZoneGroupSettings = new SelectiveSaleZoneGroup
                    {
                        SellingNumberPlanId = Context.MigrationContext.DefaultSellingNumberPlanId,
                        ZoneIds = new HashSet<long>(rules.Select(r => Convert.ToInt64(r.SaleZoneID))).ToList()
                    }
                }
            };
            return settings;
        }

        Dictionary<string, MarketPrice> GetMarketPrices(IEnumerable<SourceMarketPrice> rules)
        {
            Dictionary<string, MarketPrice> marketPrices = new Dictionary<string, MarketPrice>();

            foreach (SourceMarketPrice sourceMarketPrice in rules)
            {
                List<int> serviceIds = GetMatchedFlaggedServices(sourceMarketPrice.ServicesFlag);
                MarketPrice marketPrice = new MarketPrice
                {
                    Minimum = sourceMarketPrice.FromRate,
                    Maximum = sourceMarketPrice.ToRate,
                    ServiceIds = serviceIds
                };
                string key = string.Join(",", serviceIds);
                if (!marketPrices.ContainsKey(key))
                    marketPrices.Add(key, marketPrice);
            }

            return marketPrices;
        }

        private List<int> GetMatchedFlaggedServices(short serviceFlagId)
        {
            List<int> rslt = new List<int>();
            if (serviceFlagId == 0)
            {
                rslt.Add(_allSaleEntityServiceFlags[_olsFlaggedServiceIds[0].ToString()].ZoneServiceConfigId);
                return rslt;
            }
            foreach (short serviceId in _olsFlaggedServiceIds)
                if ((serviceId & serviceFlagId) == serviceId)
                    rslt.Add(_allSaleEntityServiceFlags[serviceId.ToString()].ZoneServiceConfigId);
            return rslt.OrderBy(s => s).ToList();
        }
    }
}
