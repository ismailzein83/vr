using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SellingNumberPlan;
using TOne.WhS.BusinessEntity.MainExtensions.SupplierZoneGroups;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules.Entities;
using Vanrise.Rules.Pricing;
using Vanrise.Rules.Pricing.MainExtensions.ExtraCharge;

namespace TOne.WhS.DBSync.Business.Migrators
{
    public class TodRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Tod"; }
        }
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly int _ruleTypeId;

        public TodRuleMigrator(RuleMigrationContext context)
            : base(context)
        {

            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            _allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;

            var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            _allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;

            ExtraChargeRuleManager manager = new ExtraChargeRuleManager();
            _ruleTypeId = manager.GetRuleTypeId();


        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> sourceRules = new List<SourceRule>();
            SourceTodDataManager dataManager = new SourceTodDataManager(Context.MigrationContext.ConnectionString);
            var todRules = dataManager.GetSourceTods();
            sourceRules = GetRules(todRules);

            return sourceRules;
        }
        List<SourceRule> GetRules(IEnumerable<SourceTod> tods)
        {
            List<SourceRule> result = new List<SourceRule>();

            var sellingRules = tods.FindAllRecords(s => s.SupplierId.Equals("SYS"));
            var purchaseRules = tods.FindAllRecords(s => !s.SupplierId.Equals("SYS"));

            var rulesDictionary = GroupTodRules(sellingRules, "Sale");

            foreach (var key in rulesDictionary.Keys)
            {
                var todRules = rulesDictionary[key];
                result.Add(GetSellingSourceRule(todRules));
            }

            rulesDictionary = GroupTodRules(purchaseRules, "Purchase");
            foreach (var key in rulesDictionary.Keys)
            {
                var todRules = rulesDictionary[key];
                result.Add(GetPurchaseSourceRule(todRules));
            }

            return result;
        }
        SourceRule GetPurchaseSourceRule(List<SourceTod> todRules)
        {
            List<long> zoneIds = new List<long>();
            SourceTod defaultTod = todRules.FirstOrDefault();
            foreach (SourceTod sourceTod in todRules)
            {
                if (!_allSupplierZones.ContainsKey(sourceTod.ZoneId.ToString()))
                    this.TotalRowsFailed++;
                else
                    zoneIds.Add(_allSupplierZones[sourceTod.ZoneId.ToString()].SupplierZoneId);
            }

            RateTypeRule extraChargeRule = new RateTypeRule
            {
                Settings = new PricingRuleRateTypeSettings
                {

                },
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                }
            };

            extraChargeRule.Criteria.FieldsValues.Add("Carriers", new BusinessEntityValues
            {
                BusinessEntityGroup = new SelectiveSupplierZoneGroup()
                {
                    SuppliersWithZones = new List<SupplierWithZones>
                    {
                        new SupplierWithZones
                        {
                            SupplierId = _allCarrierAccounts[defaultTod.SupplierId].CarrierAccountId,
                            SupplierZoneIds = zoneIds
                        }
                    }
                }
            });

            return GetSourceRule(extraChargeRule, defaultTod);
        }
        SourceRule GetSellingSourceRule(List<SourceTod> todRules)
        {
            List<long> zoneIds = new List<long>();
            SourceTod defaultTod = todRules.FirstOrDefault();
            foreach (SourceTod sourceTod in todRules)
            {
                if (!_allSaleZones.ContainsKey(sourceTod.ZoneId.ToString()))
                    this.TotalRowsFailed++;
                else
                    zoneIds.Add(_allSaleZones[sourceTod.ZoneId.ToString()].SaleZoneId);
            }

            RateTypeRule extraChargeRule = new RateTypeRule
            {
                Settings = new PricingRuleRateTypeSettings
                {

                },
                Criteria = new GenericRuleCriteria
                {

                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                }

            };

            extraChargeRule.Criteria.FieldsValues.Add("Carriers", new BusinessEntityValues
            {
                BusinessEntityGroup = new SelectiveCustomerGroup
                {
                    CustomerIds = new List<int>() { _allCarrierAccounts[defaultTod.CustomerId].CarrierAccountId },
                }
            });
            extraChargeRule.Criteria.FieldsValues.Add("Zones", new BusinessEntityValues
            {
                BusinessEntityGroup = new SelectiveSaleZoneGroup
                {
                    SellingNumberPlanId = Context.MigrationContext.DefaultSellingNumberPlanId,
                    ZoneIds = zoneIds
                }
            });

            return GetSourceRule(extraChargeRule, defaultTod);
        }
        SourceRule GetSourceRule(RateTypeRule extraChargeRule, SourceTod defaultTod)
        {
            return new SourceRule
            {
                Rule = new Rule
                {
                    RuleDetails = Serializer.Serialize(extraChargeRule),
                    TypeId = _ruleTypeId,
                    BED = defaultTod.BED,
                    EED = defaultTod.EED
                }
            };
        }
        Dictionary<string, List<SourceTod>> GroupTodRules(IEnumerable<SourceTod> sellingRules, string keyType)
        {
            Dictionary<string, List<SourceTod>> tods = new Dictionary<string, List<SourceTod>>();
            List<SourceTod> sourceTods;
            foreach (SourceTod tod in sellingRules)
            {
                string key = GetTodKey(tod, keyType);
                if (!tods.TryGetValue(key, out sourceTods))
                {
                    sourceTods = new List<SourceTod>();
                    tods.Add(key, sourceTods);
                }
                sourceTods.Add(tod);
            }

            return tods;
        }
        string GetTodKey(SourceTod tod, string type)
        {
            switch (type)
            {
                case "Sale":
                    return "";
                case "Purchase":
                    return "";
            }
            return "";
        }

    }
}
