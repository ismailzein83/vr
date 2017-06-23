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
using Vanrise.Common;

namespace Retail.EntitiesMigrator.Migrators
{
    public class OffNetRatesMigrator
    {
        public void Execute()
        {
            List<OffNetRate> offnetRates = new OffNetRatesDataManager().GetOffNetRates();
            var accountBEManager = new AccountBEManager();
            Dictionary<string, Account> operatorsByName = new Dictionary<string, Account>();
            foreach (var acc in accountBEManager.GetAccounts(Helper.OperatorBEDefinitionId).Values)
            {
                operatorsByName.Add(acc.Name, acc);
            }
            Dictionary<string, Account> branchesBySourceId = new Dictionary<string, Account>();
            foreach (var acc in accountBEManager.GetAccounts(Helper.AccountBEDefinitionId).Values)
            {
                if (acc.SourceId != null && acc.TypeId == Helper.BranchAccountTypeId)
                    branchesBySourceId.Add(acc.SourceId, acc);
            }
            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            var defaultTariffRule = Helper.CreateTariffRule(Helper.OffNetRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { FractionUnit = 60 });
            tariffRules.Add(defaultTariffRule);

            Dictionary<long, List<OffNetRate>> ratesByBranchId = new Dictionary<long, List<OffNetRate>>();

            foreach (var rate in offnetRates)
            {
                Account branch;
                if (branchesBySourceId.TryGetValue(rate.SourceBranchId, out branch))
                {
                    ratesByBranchId.GetOrCreateItem(branch.AccountId).Add(rate);
                }
            }
            foreach (var branchRates in ratesByBranchId)
            {
                long branchId = branchRates.Key;

                Dictionary<Decimal, int> rateCounts = new Dictionary<decimal, int>();
                foreach (var rate in branchRates.Value)
                {
                    if (rateCounts.ContainsKey(rate.RateDetail.Rate))
                        rateCounts[rate.RateDetail.Rate]++;
                    else
                        rateCounts.Add(rate.RateDetail.Rate, 1);
                }
                Decimal mostUsedRate = rateCounts.OrderByDescending(itm => itm.Value).First().Key;
                var defaultAccountRateRule = GetRateValueRuleDetails(null, branchRates.Key, new RateDetails { Rate = mostUsedRate });
                rateRules.Add(defaultAccountRateRule);
                foreach (var rate in branchRates.Value)
                {
                    Account oper;
                    if (operatorsByName.TryGetValue(rate.OperatorName, out oper))
                    {
                        if (rate.RateDetail.Rate != mostUsedRate)
                            rateRules.Add(GetRateValueRuleDetails(oper.AccountId, branchId, rate.RateDetail));

                        if (rate.RateDetail.FractionUnit != 60)
                            tariffRules.Add(GetTariffRuleDetails(oper.AccountId, branchId, rate.RateDetail));
                    }
                }
            }
            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);
        }

        private RateValueRule GetRateValueRuleDetails(long? operatorId, long? branchId, RateDetails rateDetails)
        {
            return Helper.CreateRateValueRule(Helper.OffNetRuleDefinition, GetCriteriaFieldsValues(operatorId, branchId), rateDetails);
        }

        private TariffRule GetTariffRuleDetails(long? operatorId, long? branchId, RateDetails rateDetails)
        {
            return Helper.CreateTariffRule(Helper.OffNetRuleDefinition, GetCriteriaFieldsValues(operatorId, branchId), rateDetails);
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(long? operatorId, long? branchId)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = GetDefaultCriteriaFieldValues();
            if (operatorId.HasValue)
                Helper.AddOperatorField(result, operatorId.Value);
            if (branchId.HasValue)
                Helper.AddAccountField(result, new List<object> { branchId.Value });

            return result;
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetDefaultCriteriaFieldValues()
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = Helper.BuildCriteriaFieldsValues(Helper.OffNetRuleDefinition.ServiceTypeId, Helper.OffNetRuleDefinition.ChargingPolicyId, MultiNet.Business.TrafficDirection.OutGoing);
            return result;
        }
    }
}
