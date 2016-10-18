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
            SourceCommissionDataManager dataManager = new SourceCommissionDataManager(Context.MigrationContext.ConnectionString);
            var commissionRules = dataManager.GetSourceCommissions();
            sourceRules = GetRules(commissionRules);
            return sourceRules;
        }
        List<SourceRule> GetRules(IEnumerable<SourceCommission> commissions)
        {
            List<SourceRule> result = new List<SourceRule>();

            var sellingRules = commissions.FindAllRecords(s => s.SupplierId.Equals("SYS"));
            var purchaseRules = commissions.FindAllRecords(s => !s.SupplierId.Equals("SYS"));

            var rulesDictionary = GroupCommissionRules(sellingRules, "Sale");

            foreach (var key in rulesDictionary.Keys)
            {
                var commissionRules = rulesDictionary[key];
                var rule = GetSellingSourceRule(commissionRules);
                if (rule != null)
                    result.Add(rule);
            }

            rulesDictionary = GroupCommissionRules(purchaseRules, "Purchase");
            foreach (var key in rulesDictionary.Keys)
            {
                var commissionRules = rulesDictionary[key];
                var rule = GetPurchaseSourceRule(commissionRules);
                if (rule != null)
                    result.Add(rule);
            }

            return result;
        }
        SourceRule GetPurchaseSourceRule(List<SourceCommission> commissionRules)
        {
            List<long> zoneIds = new List<long>();
            SourceCommission defaultCommission = commissionRules.FirstOrDefault();
            CarrierAccount supplier;
            if (!_allCarrierAccounts.TryGetValue(defaultCommission.SupplierId, out supplier))
            {
                this.TotalRowsFailed += commissionRules.Count;
                return null;
            }
            foreach (SourceCommission sourceCommission in commissionRules)
            {
                if (!_allSupplierZones.ContainsKey(sourceCommission.ZoneId.ToString()))
                    this.TotalRowsFailed++;
                else
                    zoneIds.Add(_allSupplierZones[sourceCommission.ZoneId.ToString()].SupplierZoneId);
            }

            ExtraChargeRule extraChargeRule = new ExtraChargeRule
            {

                Settings = new PricingRuleExtraChargeSettings
                {
                    Actions = GetActions(defaultCommission)
                },
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()

                },
                DefinitionId = new Guid("ADED2932-BB9F-49B7-A561-CB7C1413084F")
            };

            extraChargeRule.Criteria.FieldsValues.Add("Carriers", new BusinessEntityValues
            {
                BusinessEntityGroup = new SelectiveSupplierZoneGroup()
                {
                    SuppliersWithZones = new List<SupplierWithZones>
                    {
                        new SupplierWithZones
                        {
                            SupplierId = supplier.CarrierAccountId,
                            SupplierZoneIds = zoneIds
                        }
                    }
                }
            });

            return GetSourceRule(extraChargeRule, defaultCommission);
        }
        SourceRule GetSellingSourceRule(List<SourceCommission> commissionRules)
        {
            List<long> zoneIds = new List<long>();
            SourceCommission defaultCommission = commissionRules.FirstOrDefault();
            CarrierAccount customer;
            if (!_allCarrierAccounts.TryGetValue(defaultCommission.CustomerId, out customer))
            {
                this.TotalRowsFailed += commissionRules.Count;
                return null;
            }
            foreach (SourceCommission sourceCommission in commissionRules)
            {
                if (!_allSaleZones.ContainsKey(sourceCommission.ZoneId.ToString()))
                    this.TotalRowsFailed++;
                else
                    zoneIds.Add(_allSaleZones[sourceCommission.ZoneId.ToString()].SaleZoneId);
            }

            ExtraChargeRule extraChargeRule = new ExtraChargeRule
            {
                Settings = new PricingRuleExtraChargeSettings
                {
                    Actions = GetActions(defaultCommission)
                },
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                },
                DefinitionId = new Guid("90A47A0A-3EF9-4941-BC21-CA0BE44FC5A4")
            };

            extraChargeRule.Criteria.FieldsValues.Add("Carriers", new BusinessEntityValues
            {
                BusinessEntityGroup = new SelectiveCustomerGroup
                {
                    CustomerIds = new List<int>() { customer.CarrierAccountId },
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

            return GetSourceRule(extraChargeRule, defaultCommission);
        }
        List<PricingRuleExtraChargeActionSettings> GetActions(SourceCommission commission)
        {
            List<PricingRuleExtraChargeActionSettings> actions = new List<PricingRuleExtraChargeActionSettings>();
            if (commission.Percentage.HasValue)
                actions.Add(new PercentageExtraChargeSettings
                {
                    FromRate = commission.FromRate,
                    ToRate = commission.ToRate,
                    ExtraPercentage = commission.Percentage.Value
                });
            else if (commission.Amount.HasValue)
                actions.Add(new FixedExtraChargeSettings
                {
                    FromRate = commission.FromRate,
                    ToRate = commission.ToRate,
                    ExtraAmount = commission.Amount.Value
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
        Dictionary<string, List<SourceCommission>> GroupCommissionRules(IEnumerable<SourceCommission> sellingRules, string keyType)
        {
            Dictionary<string, List<SourceCommission>> commissions = new Dictionary<string, List<SourceCommission>>();
            List<SourceCommission> sourceCommissions;
            foreach (SourceCommission commission in sellingRules)
            {
                string key = GetCommissionKey(commission, keyType);
                if (!commissions.TryGetValue(key, out sourceCommissions))
                {
                    sourceCommissions = new List<SourceCommission>();
                    commissions.Add(key, sourceCommissions);
                }
                sourceCommissions.Add(commission);
            }

            return commissions;
        }
        string GetCommissionKey(SourceCommission commission, string type)
        {
            switch (type)
            {
                case "Sale":
                    return string.Format("{0},{1},{2},{3},{4},{5}", commission.CustomerId,
                        commission.Amount ?? commission.Percentage.Value,
                        commission.FromRate, commission.ToRate, commission.BED, commission.EED);
                case "Purchase":
                    return string.Format("{0},{1},{2},{3},{4},{5}", commission.SupplierId,
                        commission.Amount ?? commission.Percentage.Value,
                        commission.FromRate, commission.ToRate, commission.BED, commission.EED);
            }
            return "";
        }

    }
}
