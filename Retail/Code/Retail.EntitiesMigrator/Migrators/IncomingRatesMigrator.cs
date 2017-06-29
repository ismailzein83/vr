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
        public void Ececute()
        {
            IIncomingRatesDataManager dataManager = EntitiesMigratorDataManagerFactory.GetDataManager<IIncomingRatesDataManager>();
            IEnumerable<IncomingRate> incomingRates = dataManager.GetIncomingRates();

            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            rateRules.Add(Helper.CreateRateValueRule(Helper.MobileRuleDefinition, GetDefaultCriteriaFieldValues(Helper.MobileRuleDefinition), new RateDetails { Rate = 0 }));
            rateRules.Add(Helper.CreateRateValueRule(Helper.OffNetRuleDefinition, GetDefaultCriteriaFieldValues(Helper.OffNetRuleDefinition), new RateDetails { Rate = 0 }));
            rateRules.Add(Helper.CreateRateValueRule(Helper.OnNetRuleDefinition, GetDefaultCriteriaFieldValues(Helper.OnNetRuleDefinition), new RateDetails { Rate = 0 }));
            rateRules.Add(Helper.CreateRateValueRule(Helper.IntlRuleDefinition, GetDefaultCriteriaFieldValues(Helper.IntlRuleDefinition), new RateDetails { Rate = 0 }));

            tariffRules.Add(Helper.CreateTariffRule(Helper.MobileRuleDefinition, GetDefaultCriteriaFieldValues(Helper.MobileRuleDefinition), new RateDetails { FractionUnit = 60 }));
            tariffRules.Add(Helper.CreateTariffRule(Helper.OffNetRuleDefinition, GetDefaultCriteriaFieldValues(Helper.OffNetRuleDefinition), new RateDetails { FractionUnit = 60 }));
            tariffRules.Add(Helper.CreateTariffRule(Helper.OnNetRuleDefinition, GetDefaultCriteriaFieldValues(Helper.OnNetRuleDefinition), new RateDetails { FractionUnit = 60 }));
            tariffRules.Add(Helper.CreateTariffRule(Helper.IntlRuleDefinition, GetDefaultCriteriaFieldValues(Helper.IntlRuleDefinition), new RateDetails { FractionUnit = 60 }));
            HashSet<long> addedBranchIds = new HashSet<long>();
            foreach (IncomingRate incomingRate in incomingRates)
            {
                AccountBEManager accountManager = new AccountBEManager();
                Account account = accountManager.GetAccountBySourceId(Helper.AccountBEDefinitionId, Helper.GetUserSourceId(incomingRate.SubscriberId.ToString()));
                if (account != null)
                {
                    Account parentAccount = accountManager.GetSelfOrParentAccountOfType(Helper.AccountBEDefinitionId, account.AccountId, Helper.BranchAccountTypeId);
                    if (parentAccount != null && !addedBranchIds.Contains(parentAccount.AccountId))
                    {
                        addedBranchIds.Add(parentAccount.AccountId);
                        rateRules.Add(GetRateValueRuleDetails(parentAccount.AccountId, incomingRate.LocalRate, Helper.OnNetRuleDefinition, true));
                        rateRules.Add(GetRateValueRuleDetails(parentAccount.AccountId, incomingRate.LocalRate, Helper.OffNetRuleDefinition, true));
                        rateRules.Add(GetRateValueRuleDetails(parentAccount.AccountId, incomingRate.LocalRate, Helper.MobileRuleDefinition, true));

                        rateRules.Add(GetRateValueRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.OnNetRuleDefinition));
                        rateRules.Add(GetRateValueRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.OffNetRuleDefinition));
                        rateRules.Add(GetRateValueRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.MobileRuleDefinition));
                        rateRules.Add(GetRateValueRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.IntlRuleDefinition));
                        if (incomingRate.LocalRate.FractionUnit != 60)
                        {
                            tariffRules.Add(GetTariffRuleDetails(parentAccount.AccountId, incomingRate.LocalRate, Helper.OnNetRuleDefinition, true));
                            tariffRules.Add(GetTariffRuleDetails(parentAccount.AccountId, incomingRate.LocalRate, Helper.OffNetRuleDefinition, true));
                            tariffRules.Add(GetTariffRuleDetails(parentAccount.AccountId, incomingRate.LocalRate, Helper.MobileRuleDefinition, true));
                        }
                        if (incomingRate.InternationalRate.FractionUnit != 60)
                        {
                            tariffRules.Add(GetTariffRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.OnNetRuleDefinition));
                            tariffRules.Add(GetTariffRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.OffNetRuleDefinition));
                            tariffRules.Add(GetTariffRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.MobileRuleDefinition));
                            tariffRules.Add(GetTariffRuleDetails(parentAccount.AccountId, incomingRate.InternationalRate, Helper.IntlRuleDefinition));
                        }
                    }
                }
            }
            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);

        }

        private RateValueRule GetRateValueRuleDetails(long accountId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, bool isNational = false)
        {
            return Helper.CreateRateValueRule(ruleDefinitionDetails, isNational
                                                                        ? GetNationalCriteriaFieldsValues(ruleDefinitionDetails, accountId)
                                                                        : GetCriteriaFieldsValues(ruleDefinitionDetails, accountId), rateDetails);
        }
        private TariffRule GetTariffRuleDetails(long accountId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, bool isNational = false)
        {
            return Helper.CreateTariffRule(ruleDefinitionDetails, isNational
                                                                        ? GetNationalCriteriaFieldsValues(ruleDefinitionDetails, accountId)
                                                                        : GetCriteriaFieldsValues(ruleDefinitionDetails, accountId), rateDetails);
        }
        private Dictionary<string, GenericRuleCriteriaFieldValues> GetDefaultCriteriaFieldValues(RuleDefinitionDetails ruleDefinitionDetails)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = Helper.BuildCriteriaFieldsValues(ruleDefinitionDetails.ServiceTypeId, ruleDefinitionDetails.ChargingPolicyId, MultiNet.Business.TrafficDirection.InComming);
            return result;
        }
        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(RuleDefinitionDetails ruleDefinitionDetails, long accountId)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = GetDefaultCriteriaFieldValues(ruleDefinitionDetails);
            Helper.AddAccountField(result, new List<object> { accountId });

            return result;
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetNationalCriteriaFieldsValues(RuleDefinitionDetails ruleDefinitionDetails, long accountId)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = GetCriteriaFieldsValues(ruleDefinitionDetails, accountId);
            Helper.AddIsSameZoneField(result, 1);

            return result;
        }



    }
}
