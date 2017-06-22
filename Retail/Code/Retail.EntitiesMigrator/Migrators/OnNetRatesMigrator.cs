using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
using Vanrise.Rules.Pricing.MainExtensions.RateValue;
using Vanrise.Rules.Pricing.MainExtensions.Tariff;

namespace Retail.EntitiesMigrator.Migrators
{
    public class OnNetRatesMigrator
    {
        int _OnNetChargingPolicy;

        public OnNetRatesMigrator(int chargingPolicy)
        {
            _OnNetChargingPolicy = chargingPolicy;

        }
        public void Execute(Guid accountBEDefinitionId)
        {
            IEnumerable<InternationalRate> internationalRates = GetRates();
            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            foreach (InternationalRate internationalRate in internationalRates)
            {

                AccountBEManager accountManager = new AccountBEManager();
                Account account = accountManager.GetAccountBySourceId(accountBEDefinitionId, internationalRate.SubscriberId.ToString());
                if (account != null)
                {
                    rateRules.Add(GetGenereicRule(RuleType.Rate, account.AccountId, internationalRate.InternationalRateDetail, internationalRate.ActivationDate, Helper.OnNetRuleDefinition));
                    if (internationalRate.InternationalRateDetail.FractionUnit != 60)
                        tariffRules.Add(GetGenereicRule(RuleType.Tariff, account.AccountId, internationalRate.InternationalRateDetail, internationalRate.ActivationDate, Helper.OnNetRuleDefinition));

                }

            }
            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);

        }

        private GenericRule GetGenereicRule(RuleType ruleType, long accontId, RateDetails rateDetails, DateTime bed, Entities.RuleDefinitionDetails ruleDefinitionDetails)
        {
            switch (ruleType)
            {
                case RuleType.Rate:
                    return GetRateValueRuleDetails(accontId, rateDetails, ruleDefinitionDetails, bed);

                case RuleType.Tariff:
                    return GetTariffRuleDetails(accontId, rateDetails, ruleDefinitionDetails, bed);
            }
            return null;
        }

        private RateValueRule GetRateValueRuleDetails(long accountId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, DateTime bed)
        {
            RateValueRule ruleDetails = new RateValueRule
            {
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = GetCriteriaFieldsValues(ruleDefinitionDetails, new List<object> { accountId })
                },
                Settings = new FixedRateValueSettings
                {
                    CurrencyId = Helper.CurrencyId,
                    NormalRate = rateDetails.Rate
                },
                DefinitionId = ruleDefinitionDetails.RateDefinitionId,
                Description = "Migrated OnNet Rate Rule",
                BeginEffectiveTime = bed

            };
            return ruleDetails;
        }

        private TariffRule GetTariffRuleDetails(long accountId, RateDetails rateDetails, RuleDefinitionDetails ruleDefinitionDetails, DateTime bed)
        {
            TariffRule ruleDetails = new TariffRule
            {
                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = GetCriteriaFieldsValues(ruleDefinitionDetails, new List<object> { accountId })
                },
                Settings = new RegularTariffSettings
                {
                    CurrencyId = Helper.CurrencyId,
                    FractionUnit = rateDetails.FractionUnit,
                    PricingUnit = 60
                },
                DefinitionId = ruleDefinitionDetails.TariffDefinitionId,
                Description = "Migrated OnNet Tariff Rule",
                BeginEffectiveTime = bed

            };
            return ruleDetails;
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(RuleDefinitionDetails ruleDefinitionDetails, List<object> accountIds)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = new Dictionary<string, GenericRuleCriteriaFieldValues>();

            Helper.AddDirectionField(result, TrafficDirection.OutGoing);
            Helper.AddServiceTypeField(result, ruleDefinitionDetails.ServiceTypeId);
            Helper.AddChargingPolicyField(result, _OnNetChargingPolicy);
            Helper.AddAccountField(result, accountIds);

            return result;
        }

        IEnumerable<InternationalRate> GetRates()
        {
            List<InternationalRate> result = new List<InternationalRate>();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RatesDBConnString"].ConnectionString))
            {
                if (conn.State == System.Data.ConnectionState.Closed)
                    conn.Open();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = query_GetONNet;
                command.CommandType = System.Data.CommandType.Text;

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    result.Add(new InternationalRate
                    {
                        ActivationDate = new DateTime(2010, 01, 01),
                        SubscriberId = (long)reader["AC_ACCOUNTNO"],
                        InternationalRateDetail = ParseRateDetails(reader["DR_RATECONF"] as string)
                    });
                }
                reader.Close();
                conn.Close();
            }

            return result;

        }

        private RateDetails ParseRateDetails(string rateString)
        {
            string[] rateStringDetails = rateString.Split('*');
            return new RateDetails
            {
                FractionUnit = int.Parse(rateStringDetails[1]),
                Rate = decimal.Parse(rateStringDetails[2])
            };
        }

        const string query_GetONNet = @"
SELECT distinct [AC_ACCOUNTNO]
     
      ,[DR_RATECONF]
  FROM [vw_Multinet_OnNet_Rates]
";

    }
}
