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
    public class TariffRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Tod"; }
        }
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly int _ruleTypeId;

        public TariffRuleMigrator(RuleMigrationContext context)
            : base(context)
        {

            //var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            //_allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            //var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            //_allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;

            //var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            //_allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;

            TariffRuleManager manager = new TariffRuleManager();
            _ruleTypeId = manager.GetRuleTypeId();


        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> sourceRules = new List<SourceRule>();
            sourceRules.Add(GetDefaultSaleTariffRule());
            sourceRules.Add(GetDefaultSupplierTariffRule());
            return sourceRules;
        }

        SourceRule GetDefaultSupplierTariffRule()
        {
            TariffRule supplierTariffRule = new TariffRule
            {
                BeginEffectiveTime = RuleMigrator.s_defaultRuleBED,
                EndEffectiveTime = null,
                Settings = new Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings
                {
                    PricingUnit = 60,
                    FractionUnit = 0,
                    CurrencyId = Context.CurrencyId
                },
                DefinitionId = new Guid("5AEB0DAD-4BB8-44B4-ACBE-C8C917E88B58"),
                Description = "Default Supplier Tariff Rule"
            };
            SourceRule defaultRouteRule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = RuleMigrator.s_defaultRuleBED,
                    EED = null,
                    TypeId = _ruleTypeId,
                    RuleDetails = Serializer.Serialize(supplierTariffRule)
                }
            };

            return defaultRouteRule;
        }

        SourceRule GetDefaultSaleTariffRule()
        {
            TariffRule saleTariffRule = new TariffRule
            {
                BeginEffectiveTime = RuleMigrator.s_defaultRuleBED,
                EndEffectiveTime = null,
                Settings = new Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings
                {
                    PricingUnit = 60,
                    FractionUnit = 0,
                    CurrencyId = Context.CurrencyId
                },
                DefinitionId = new Guid("F24CB510-0B65-48C8-A723-1F6EBFEEA9E8"),
                Description = "Default Sale Tariff Rule"
            };

            SourceRule defaultRouteRule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = RuleMigrator.s_defaultRuleBED,
                    EED = null,
                    TypeId = _ruleTypeId,
                    RuleDetails = Serializer.Serialize(saleTariffRule)
                }
            };

            return defaultRouteRule;
        }

    }
}
