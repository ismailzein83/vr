using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SellingNumberPlan;
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

        private List<SourceRule> GetRules(IEnumerable<SourceCommission> commissions)
        {
            List<SourceRule> result = new List<SourceRule>();

            var sellingRules = commissions.FindAllRecords(s => s.SupplierId.Equals("SYS"));
            var purchaseRules = commissions.FindAllRecords(s => !s.SupplierId.Equals("SYS"));

            var rulesDictionary = GroupCommissionRules(sellingRules, "Sale");

            foreach (var key in rulesDictionary.Keys)
            {
                var commissionRules = rulesDictionary[key];
                result.Add(GetSellingSourceRule(commissionRules));
            }

            rulesDictionary = GroupCommissionRules(purchaseRules, "Purchase");
            foreach (var key in rulesDictionary.Keys)
            {
                var commissionRules = rulesDictionary[key];
                result.Add(GetPurchaseSourceRule(commissionRules));
            }


            SourceRule rule = new SourceRule
            {
                Rule = new Rule
                {

                }
            };
            return new List<SourceRule>();
        }

        private SourceRule GetPurchaseSourceRule(List<SourceCommission> commissionRules)
        {
            throw new NotImplementedException();
        }

        private SourceRule GetSellingSourceRule(List<SourceCommission> commissionRules)
        {
            List<long> zoneIds = new List<long>();
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
                    Actions = new List<PricingRuleExtraChargeActionSettings>()
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
                    CustomerIds = new List<int>() { _allCarrierAccounts[commissionRules.First().CustomerId].CarrierAccountId },
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

            return new SourceRule
            {
                Rule = new Rule
                {
                    RuleDetails = Serializer.Serialize(extraChargeRule)
                }
            };
        }

        private Dictionary<string, List<SourceCommission>> GroupCommissionRules(IEnumerable<SourceCommission> sellingRules, string keyType)
        {
            Dictionary<string, List<SourceCommission>> commissions = new Dictionary<string, List<SourceCommission>>();
            List<SourceCommission> sourceCommissions;
            foreach (SourceCommission commission in sellingRules)
            {
                string key = GetCommissionKey(commission, keyType);
                if (!commissions.TryGetValue(key, out sourceCommissions))
                {
                    commissions.Add(key, sourceCommissions);
                }
                sourceCommissions.Add(commission);
            }

            return commissions;
        }

        private string GetCommissionKey(SourceCommission commission, string type)
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
