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
    public class MobileRatesMigrator
    {

        public MobileRatesMigrator()
        {
        }
        public void Execute()
        {
            IEnumerable<InternationalRate> internationalRates = GetRates();
            List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();
            //var defaultRateRule = Helper.CreateRateValueRule(Helper.MobileRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { Rate = 0 });
            var defaultTariffRule = Helper.CreateTariffRule(Helper.MobileRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { FractionUnit = 60 });
            tariffRules.Add(defaultTariffRule);
            //rateRules.Add(defaultRateRule);
            HashSet<long> addedAccounts = new HashSet<long>();
            foreach (InternationalRate internationalRate in internationalRates)
            {

                AccountBEManager accountManager = new AccountBEManager();
                Account account = accountManager.GetAccountBySourceId(Helper.AccountBEDefinitionId, Helper.GetBranchSourceId(internationalRate.SubscriberId.ToString()));
                if (account != null && !addedAccounts.Contains(account.AccountId))
                {
                    addedAccounts.Add(account.AccountId);
                    rateRules.Add(GetRateValueRuleDetails(account.AccountId, internationalRate.InternationalRateDetail));
                    if (internationalRate.InternationalRateDetail.FractionUnit != 60)
                        tariffRules.Add(GetTariffRuleDetails(account.AccountId, internationalRate.InternationalRateDetail));
                }

            }

            Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);

        }
        IEnumerable<InternationalRate> GetRates()
        {
            Dictionary<long, InternationalRate> result = new Dictionary<long, InternationalRate>();
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
                    long subscriberId = (long)reader["AC_ACCOUNTNO"];
                    if (!result.ContainsKey(subscriberId))
                    {
                        result.Add(subscriberId, new InternationalRate
                        {
                            ActivationDate = new DateTime(2010, 01, 01),
                            SubscriberId = (long)reader["AC_ACCOUNTNO"],
                            InternationalRateDetail = ParseRateDetails(reader["DR_RATECONF"] as string)
                        });
                    }
                }
                reader.Close();
                conn.Close();
            }

            return result.Values;

        }
        private RateValueRule GetRateValueRuleDetails(long accountId, RateDetails rateDetails)
        {
            return Helper.CreateRateValueRule(Helper.MobileRuleDefinition, GetCriteriaFieldsValues(accountId), rateDetails);
        }
        private TariffRule GetTariffRuleDetails(long accountId, RateDetails rateDetails)
        {
            return Helper.CreateTariffRule(Helper.MobileRuleDefinition, GetCriteriaFieldsValues(accountId), rateDetails);
        }
        private Dictionary<string, GenericRuleCriteriaFieldValues> GetDefaultCriteriaFieldValues()
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = Helper.BuildCriteriaFieldsValues(Helper.MobileRuleDefinition.ServiceTypeId, Helper.MobileRuleDefinition.ChargingPolicyId, MultiNet.Business.TrafficDirection.OutGoing);
            return result;
        }
        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(long accountId)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = GetDefaultCriteriaFieldValues();
            Helper.AddAccountField(result, new List<object> { accountId });

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
  FROM [vw_Multinet_Mobile_Rates]
";

    }
}
