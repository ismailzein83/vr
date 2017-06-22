using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using Retail.EntitiesMigrator.Data.SQL;
using Retail.EntitiesMigrator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules.Pricing.MainExtensions.RateValue;

namespace Retail.EntitiesMigrator.Migrators
{
    public class OffNetRatesMigrator
    {
        public void Execute()
        {
            List<OffNetRate> offnetRates = new OffNetRatesDataManager().GetOffNetRates();
            Dictionary<string, Account> operatorsByName = new Dictionary<string, Account>();
            foreach (var acc in new AccountBEManager().GetAccounts(Helper.OperatorBEDefinitionId).Values)
            {
                operatorsByName.Add(acc.Name, acc);
            }
            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            var defaultTariffRule = Helper.CreateTariffRule(Helper.OffNetRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { FractionUnit = 60 });
            tariffRules.Add(defaultTariffRule);
            foreach (var rate in offnetRates)
            {
                Account oper;
                if (operatorsByName.TryGetValue(rate.OperatorName, out oper))
                {
                    rateRules.Add(GetRateValueRuleDetails(oper.AccountId, rate.RateDetail));
                    if (rate.RateDetail.FractionUnit != 60)
                        tariffRules.Add(GetTariffRuleDetails(oper.AccountId, rate.RateDetail));
                }
            }
            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);
        }

        private RateValueRule GetRateValueRuleDetails(long operatorId, RateDetails rateDetails)
        {
            return Helper.CreateRateValueRule(Helper.OffNetRuleDefinition, GetCriteriaFieldsValues(operatorId), rateDetails);
        }

        private TariffRule GetTariffRuleDetails(long operatorId, RateDetails rateDetails)
        {
            return Helper.CreateTariffRule(Helper.OffNetRuleDefinition, GetCriteriaFieldsValues(operatorId), rateDetails);
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(long operatorId)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = GetDefaultCriteriaFieldValues();
            Helper.AddOperatorField(result, operatorId);

            return result;
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetDefaultCriteriaFieldValues()
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = Helper.BuildCriteriaFieldsValues(Helper.OffNetRuleDefinition.ServiceTypeId, Helper.OffNetRuleDefinition.ChargingPolicyId, MultiNet.Business.TrafficDirection.OutGoing);
            return result;
        }
    }
}
