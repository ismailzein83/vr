using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.EntitiesMigrator.Data;
using Retail.EntitiesMigrator.Entities;
using Retail.MultiNet.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.NumberingPlan.Entities;
using Vanrise.Rules.Pricing.MainExtensions.RateValue;
using Vanrise.Rules.Pricing.MainExtensions.Tariff;

namespace Retail.EntitiesMigrator.Migrators
{
    public class InternationalRatesMigrator
    {
        Dictionary<string, SaleZone> _Zones;
        int _IntlChargingPolicy;
        public InternationalRatesMigrator(Dictionary<string, SaleZone> zones, int chargingPolicy)
        {
            _IntlChargingPolicy = chargingPolicy;
            _Zones = zones;
        }

        public void MigrateInternationalRates()
        {
            IInternationalRateDataManager dataManager = EntitiesMigratorDataManagerFactory.GetDataManager<IInternationalRateDataManager>();
            IEnumerable<InternationalRate> internationalRates = dataManager.GetInternationalRates();

            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            foreach (InternationalRate internationalRate in internationalRates)
            {

                SaleZone saleZone;
                if (_Zones.TryGetValue(internationalRate.ZoneName, out saleZone))
                {
                    rateRules.Add(GetGenereicRule(RuleType.Rate, saleZone.SaleZoneId, internationalRate.InternationalRateDetail, internationalRate.ActivationDate, Helper.IntlRuleDefinition));
                    if (internationalRate.InternationalRateDetail.FractionUnit != 60)
                        tariffRules.Add(GetGenereicRule(RuleType.Tariff, saleZone.SaleZoneId, internationalRate.InternationalRateDetail, internationalRate.ActivationDate, Helper.IntlRuleDefinition));
                }


            }
            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);

        }

        private GenericRule GetGenereicRule(RuleType ruleType, long zoneId, RateDetails rateDetails, DateTime bed, Entities.RuleDefinitionDetails ruleDefinitionDetails)
        {
            switch (ruleType)
            {
                case RuleType.Rate:
                    return GetRateValueRuleDetails(zoneId, rateDetails, ruleDefinitionDetails, bed);

                case RuleType.Tariff:
                    return GetTariffRuleDetails(zoneId, rateDetails, ruleDefinitionDetails, bed);

            }
            return null;
        }

        private RateValueRule GetRateValueRuleDetails(long zoneId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, DateTime bed)
        {
            RateValueRule ruleDetails = new RateValueRule
            {
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = GetCriteriaFieldsValues(ruleDefinitionDetails, new List<object> { zoneId })
                },
                Settings = new FixedRateValueSettings
                {
                    CurrencyId = Helper.CurrencyId,
                    NormalRate = rateDetails.Rate
                },
                DefinitionId = ruleDefinitionDetails.RateDefinitionId,
                Description = "Migrated International Rate Rule",
                BeginEffectiveTime = bed

            };
            return ruleDetails;
        }

        private TariffRule GetTariffRuleDetails(long zoneId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, DateTime bed)
        {
            TariffRule ruleDetails = new TariffRule
            {
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = GetCriteriaFieldsValues(ruleDefinitionDetails, new List<object> { zoneId })
                },
                Settings = new RegularTariffSettings
                {
                    CurrencyId = Helper.CurrencyId,
                    FractionUnit = rateDetails.FractionUnit,
                    PricingUnit = 60,
                    FirstPeriodRateType = FirstPeriodRateType.EffectiveRate
                },
                DefinitionId = ruleDefinitionDetails.TariffDefinitionId,
                Description = "Migrated International Tariff Rule",
                BeginEffectiveTime = bed

            };
            return ruleDetails;
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(RuleDefinitionDetails ruleDefinitionDetails, List<object> zoneIds)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = new Dictionary<string, GenericRuleCriteriaFieldValues>();

            Helper.AddDirectionField(result, TrafficDirection.OutGoing);
            Helper.AddServiceTypeField(result, ruleDefinitionDetails.ServiceTypeId);
            Helper.AddChargingPolicyField(result, _IntlChargingPolicy);
            Helper.AddZoneField(result, zoneIds);

            return result;
        }

    }
}
