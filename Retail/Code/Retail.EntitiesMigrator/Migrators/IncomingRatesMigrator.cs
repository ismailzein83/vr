using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.EntitiesMigrator.Data;
using Retail.EntitiesMigrator.Entities;
using Retail.MultiNet.Business;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules.Entities;
using Vanrise.Rules.Pricing.MainExtensions.RateValue;
using Vanrise.Rules.Pricing.MainExtensions.Tariff;

namespace Retail.EntitiesMigrator.Migrators
{
    public class IncomingRatesMigrator
    {
        public List<RuleDefinitionDetails> RuleDefinitionDetails { get; set; }

        public IncomingRatesMigrator(List<RuleDefinitionDetails> ruleDefinitions)
        {
            RuleDefinitionDetails = ruleDefinitions;
        }

        public void MigrateIncomingRates(Guid accountBEDefinitionId, Guid accountTypeId)
        {
            IIncomingRatesDataManager dataManager = EntitiesMigratorDataManagerFactory.GetDataManager<IIncomingRatesDataManager>();
            IEnumerable<IncomingRate> incomingRates = dataManager.GetIncomingRates();

            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            foreach (IncomingRate incomingRate in incomingRates)
            {
                AccountBEManager accountManager = new AccountBEManager();
                Account account = accountManager.GetAccountBySourceId(accountBEDefinitionId, incomingRate.SubscriberId.ToString());
                if (account != null)
                {
                    Account parentAccount = accountManager.GetSelfOrParentAccountOfType(accountBEDefinitionId, account.AccountId, accountTypeId);
                    if (parentAccount != null)
                    {
                        foreach (RuleDefinitionDetails ruleDefinitionDetails in RuleDefinitionDetails)
                        {
                            if (!ruleDefinitionDetails.IsInternational)
                            {
                                rateRules.Add(GetGenereicRule(RuleType.Rate, parentAccount.AccountId, incomingRate.LocalRate, incomingRate.ActivationDate, ruleDefinitionDetails, true));
                                tariffRules.Add(GetGenereicRule(RuleType.Tariff, parentAccount.AccountId, incomingRate.LocalRate, incomingRate.ActivationDate, ruleDefinitionDetails, true));
                            }
                            rateRules.Add(GetGenereicRule(RuleType.Rate, parentAccount.AccountId, incomingRate.InternationalRate, incomingRate.ActivationDate, ruleDefinitionDetails, false));
                            tariffRules.Add(GetGenereicRule(RuleType.Tariff, parentAccount.AccountId, incomingRate.InternationalRate, incomingRate.ActivationDate, ruleDefinitionDetails, false));
                        }
                    }
                }
            }
            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);

        }

        private GenericRule GetGenereicRule(RuleType ruleType, long accountId, RateDetails rateDetails, DateTime bed, Entities.RuleDefinitionDetails ruleDefinitionDetails, bool isNational)
        {
            switch (ruleType)
            {
                case RuleType.Rate:
                    return GetRateValueRuleDetails(accountId, rateDetails, ruleDefinitionDetails, bed, isNational);

                case RuleType.Tariff:
                    return GetTariffRuleDetails(accountId, rateDetails, ruleDefinitionDetails, bed, isNational);

            }
            return null;
        }

        private RateValueRule GetRateValueRuleDetails(long accountId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, DateTime bed, bool isNational)
        {
            RateValueRule ruleDetails = new RateValueRule
            {
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = GetCriteriaFieldsValues(accountId, isNational, ruleDefinitionDetails)
                },
                Settings = new FixedRateValueSettings
                {
                    CurrencyId = Helper.CurrencyId,
                    NormalRate = rateDetails.Rate
                },
                DefinitionId = ruleDefinitionDetails.RateDefinitionId,
                Description = "Migrated Incoming Rate Rule",
                BeginEffectiveTime = bed

            };
            return ruleDetails;
        }

        private TariffRule GetTariffRuleDetails(long accountId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, DateTime bed, bool isNational)
        {
            TariffRule ruleDetails = new TariffRule
            {
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = GetCriteriaFieldsValues(accountId, isNational, ruleDefinitionDetails)
                },
                Settings = new RegularTariffSettings
                {
                    CurrencyId = Helper.CurrencyId,
                    FractionUnit = rateDetails.FractionUnit,
                    PricingUnit = 60
                },
                DefinitionId = ruleDefinitionDetails.TariffDefinitionId,
                Description = "Migrated Incoming Tariff Rule",
                BeginEffectiveTime = bed

            };
            return ruleDetails;
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(long accountId, bool isNational, RuleDefinitionDetails ruleDefinitionDetails)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = new Dictionary<string, GenericRuleCriteriaFieldValues>();

            Helper.AddAccountField(result, new List<object> { accountId });
            Helper.AddDirectionField(result, TrafficDirection.InComming);
            Helper.AddServiceTypeField(result, ruleDefinitionDetails.ServiceTypeId);
            Helper.AddChargingPolicyField(result, ruleDefinitionDetails.ChargingPolicyId);
            if (isNational)
            {
                Helper.AddIsSameZoneField(result, 1);
            }

            return result;
        }
    }
}
