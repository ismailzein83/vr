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
    public class CommissionRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Commission"; }
        }
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly int _ruleTypeId;

        public CommissionRuleMigrator(RuleMigrationContext context)
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
            SourceCommissionDataManager dataManager = new SourceCommissionDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            var commissionRules = dataManager.GetSourceCommissions();
            sourceRules = GetRules(commissionRules);
            return sourceRules;
        }
        List<SourceRule> GetRules(IEnumerable<SourceCommission> commissions)
        {
            List<SourceRule> result = new List<SourceRule>();

            var sellingRules = commissions.FindAllRecords(s => s.SupplierId.Equals("SYS"));
            var purchaseRules = commissions.FindAllRecords(s => !s.SupplierId.Equals("SYS"));

            var rulesDictionary = GroupCommissionRules(sellingRules, RuleType.Sale);

            foreach (var key in rulesDictionary.Keys)
            {
                var commissionRules = rulesDictionary[key];
                var rule = GetSellingSourceRule(commissionRules, true);
                if (rule != null)
                    result.Add(rule);
            }

            rulesDictionary = GroupCommissionRules(purchaseRules, RuleType.Purchase);
            foreach (var key in rulesDictionary.Keys)
            {
                var commissionRules = rulesDictionary[key];
                var rule = GetPurchaseSourceRule(commissionRules, true);
                if (rule != null)
                    result.Add(rule);
            }

            return result;
        }

        SourceRule GetPurchaseSourceRule(List<SourceCommission> commissionRules, bool includeZonesCriteria)
        {
            List<long> zoneIds = new List<long>();
            SourceCommission defaultCommission = commissionRules.FirstOrDefault();
            CarrierAccount supplier;
            if (!_allCarrierAccounts.TryGetValue(defaultCommission.SupplierId, out supplier))
            {
                this.TotalRowsFailed += commissionRules.Count;
                Context.MigrationContext.WriteWarning(string.Format("Failed migrating Commission, Source Id: {0}", defaultCommission.CommissionId));
                return null;
            }

            ExtraChargeRule extraChargeRule = new ExtraChargeRule
            {

                Settings = new PricingRuleExtraChargeSettings
                {
                    Actions = GetActions(defaultCommission, RuleType.Purchase),
                    CurrencyId = Context.CurrencyId
                },
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()

                },
                DefinitionId = new Guid("ADED2932-BB9F-49B7-A561-CB7C1413084F"),
                Description = string.Format("Migrated Purchase Extra Charge Rule {0}", Context.Counter++),
                BeginEffectiveTime = defaultCommission.BED,
                EndEffectiveTime = defaultCommission.EED
            };

            extraChargeRule.Criteria.FieldsValues.Add("SupplierId", new StaticValues
            {
                Values = new List<object>() { supplier.CarrierAccountId }
            });

            if (includeZonesCriteria)
            {
                foreach (SourceCommission sourceCommission in commissionRules)
                {
                    if (!_allSupplierZones.ContainsKey(sourceCommission.ZoneId.ToString()))
                    {
                        Context.MigrationContext.WriteWarning(string.Format("Failed migrating Commission, Source Id: {0}", sourceCommission.CommissionId));
                        this.TotalRowsFailed++;
                    }
                    else
                        zoneIds.Add(_allSupplierZones[sourceCommission.ZoneId.ToString()].SupplierZoneId);
                }
                if (zoneIds.Count == 0)
                    return null;

                extraChargeRule.Criteria.FieldsValues.Add("SupplierZoneId", new StaticValues
                {
                    Values = zoneIds.Cast<Object>().ToList()
                });
            }

            return GetSourceRule(extraChargeRule, defaultCommission);
        }

        SourceRule GetSellingSourceRule(List<SourceCommission> commissionRules, bool includeZones)
        {
            List<long> zoneIds = new List<long>();
            SourceCommission defaultCommission = commissionRules.FirstOrDefault();
            CarrierAccount customer;
            if (!_allCarrierAccounts.TryGetValue(defaultCommission.CustomerId, out customer))
            {
                this.TotalRowsFailed += commissionRules.Count;
                return null;
            }

            ExtraChargeRule extraChargeRule = new ExtraChargeRule
            {
                Settings = new PricingRuleExtraChargeSettings
                {
                    Actions = GetActions(defaultCommission, RuleType.Sale),
                    CurrencyId = Context.CurrencyId
                },
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                DefinitionId = new Guid("90A47A0A-3EF9-4941-BC21-CA0BE44FC5A4"),
                Description = string.Format("Migrated Sale Extra Charge Rule {0}", Context.Counter++),
                BeginEffectiveTime = defaultCommission.BED,
                EndEffectiveTime = defaultCommission.EED
            };

            extraChargeRule.Criteria.FieldsValues.Add("CustomerId", new StaticValues
            {
                Values = new List<object>() { customer.CarrierAccountId }
            });

            if (includeZones)
            {
                foreach (SourceCommission sourceCommission in commissionRules)
                {
                    if (!_allSaleZones.ContainsKey(sourceCommission.ZoneId.ToString()))
                        this.TotalRowsFailed++;
                    else
                        zoneIds.Add(_allSaleZones[sourceCommission.ZoneId.ToString()].SaleZoneId);
                }
                if (zoneIds.Count == 0)
                    return null;

                extraChargeRule.Criteria.FieldsValues.Add("SaleZoneId", new StaticValues
                {
                    Values = zoneIds.Cast<Object>().ToList()
                });
            }
            return GetSourceRule(extraChargeRule, defaultCommission);
        }

        List<PricingRuleExtraChargeActionSettings> GetActions(SourceCommission commission, RuleType type)
        {
            List<PricingRuleExtraChargeActionSettings> actions = new List<PricingRuleExtraChargeActionSettings>();
            if (commission.Percentage.HasValue)
                actions.Add(new PercentageExtraChargeSettings
                {
                    FromRate = commission.FromRate,
                    ToRate = commission.ToRate,
                    ExtraPercentage = type == RuleType.Sale && Context.MigrationContext.IsCustomerCommissionNegative ? -commission.Percentage.Value : commission.Percentage.Value
                });
            else if (commission.Amount.HasValue)
                actions.Add(new FixedExtraChargeSettings
                {
                    FromRate = commission.FromRate,
                    ToRate = commission.ToRate,
                    ExtraAmount = type == RuleType.Sale && Context.MigrationContext.IsCustomerCommissionNegative ? -commission.Amount.Value : commission.Amount.Value
                });
            return actions;
        }
        SourceRule GetSourceRule(ExtraChargeRule extraChargeRule, SourceCommission defaultCommission)
        {
            return new SourceRule
            {
                Rule = new Rule
                {
                    RuleDetails = Serializer.Serialize(extraChargeRule),
                    TypeId = _ruleTypeId,
                    BED = defaultCommission.BED,
                    EED = defaultCommission.EED
                }
            };
        }
        Dictionary<string, List<SourceCommission>> GroupCommissionRules(IEnumerable<SourceCommission> sellingRules, RuleType type)
        {
            Dictionary<string, List<SourceCommission>> commissions = new Dictionary<string, List<SourceCommission>>();
            List<SourceCommission> sourceCommissions;
            foreach (SourceCommission commission in sellingRules)
            {
                string key = GetCommissionKey(commission, type);
                if (!commissions.TryGetValue(key, out sourceCommissions))
                {
                    sourceCommissions = new List<SourceCommission>();
                    commissions.Add(key, sourceCommissions);
                }
                sourceCommissions.Add(commission);
            }

            return commissions;
        }
        string GetCommissionKey(SourceCommission commission, RuleType type)
        {
            switch (type)
            {
                case RuleType.Sale:
                    return string.Format("{0},{1},{2},{3},{4},{5}", commission.CustomerId,
                        commission.Amount ?? commission.Percentage.Value,
                        commission.FromRate, commission.ToRate, commission.BED, commission.EED);
                case RuleType.Purchase:
                    return string.Format("{0},{1},{2},{3},{4},{5}", commission.SupplierId,
                        commission.Amount ?? commission.Percentage.Value,
                        commission.FromRate, commission.ToRate, commission.BED, commission.EED);
            }
            return "";
        }

    }

}
