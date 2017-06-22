﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Retail.MultiNet.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules.Entities;
using Retail.EntitiesMigrator.Entities;

namespace Retail.EntitiesMigrator
{
    public enum RuleType { Rate, Tariff }
    public class Helper
    {
        public static int NumberPlanId = 1;
        public static int CurrencyId = 1;

        public static Guid InternationalServiceTypeId = new Guid("dc1e29af-a172-4539-88ab-24210d7b0fea");
        public static Guid MobileServiceTypeId = new Guid("e99f5894-9175-421a-b0b9-e0c8d6efa126");
        public static Guid OnNetServiceTypeId = new Guid("6423ac08-170f-4775-bcaa-211dec0b56a9");
        public static Guid OffNetServiceTypeId = new Guid("6d283ee4-6d69-40df-82c8-28eca5df5407");


         public static RuleDefinitionDetails MobileRuleDefinition = new RuleDefinitionDetails
            {
                Name = "Mobile",
                RateDefinitionId = new Guid("f2348c97-8a9c-48db-9135-b0f93d2ce664"),
                TariffDefinitionId = new Guid("8d7e87f1-c91c-4a60-b404-ad4b8b85fa14"),
                IsInternational = false,
                ServiceTypeId = Helper.MobileServiceTypeId
            };

            public static RuleDefinitionDetails OnNetRuleDefinition = new RuleDefinitionDetails
            {
                Name = "On-Net",
                RateDefinitionId = new Guid("c97f4161-0fd2-4d30-acc3-06df03bf6aad"),
                TariffDefinitionId = new Guid("800c58a6-c2b3-4d4d-9777-99907bb2a6f8"),
                IsInternational = false,
                ServiceTypeId = Helper.OnNetServiceTypeId
            };

        public static RuleDefinitionDetails OffNetRuleDefinition             =new RuleDefinitionDetails
            {
                Name = "Off-Net",
                RateDefinitionId = new Guid("ff701206-e2f3-44e2-b941-6d5a848ea123"),
                TariffDefinitionId = new Guid("c795feec-ae04-4ad4-8f6b-e022c78070da"),
                IsInternational = false,
                ServiceTypeId = Helper.OffNetServiceTypeId
            };
        public static RuleDefinitionDetails IntlRuleDefinition = new RuleDefinitionDetails
            {
                Name = "International",
                RateDefinitionId = new Guid("d9cd209f-346b-4460-99da-68e8ad37bbc1"),
                TariffDefinitionId = new Guid("ecfbb042-9b68-4c84-9ab9-1e01b50b92ac"),
                IsInternational = true,
                ServiceTypeId = Helper.InternationalServiceTypeId
            };
        public static void SaveTariffRules(List<GenericRule> tariffRules)
        {
            GenericRuleManager<TariffRule> manager = new GenericRuleManager<TariffRule>();
            foreach (TariffRule rule in tariffRules)
            {
                manager.AddRule(rule);
            }
        }
        public static void SaveRateValueRules(List<GenericRule> rateValueRules)
        {
            GenericRuleManager<RateValueRule> manager = new GenericRuleManager<RateValueRule>();
            foreach (RateValueRule rule in rateValueRules)
            {
                manager.AddRule(rule);
            }
        }

        public static void AddChargingPolicyField(Dictionary<string, GenericRuleCriteriaFieldValues> criteriaFields, long policyId)
        {
            criteriaFields.Add("ChargingPolicy", new StaticValues
            {

                Values = new List<object> { policyId }
            });
        }

        public static void AddServiceTypeField(Dictionary<string, GenericRuleCriteriaFieldValues> criteriaFields, Guid serviceTypeId)
        {
            criteriaFields.Add("ServiceType", new StaticValues
            {

                Values = new List<object> { serviceTypeId }
            });
        }

        public static void AddDirectionField(Dictionary<string, GenericRuleCriteriaFieldValues> criteriaFields, TrafficDirection direction)
        {
            criteriaFields.Add("Direction", new StaticValues
            {

                Values = new List<object> { (long)direction }
            });
        }

        public static void AddAccountField(Dictionary<string, GenericRuleCriteriaFieldValues> criteriaFields, List<object> accountIds)
        {
            criteriaFields.Add("Account", new StaticValues
            {

                Values = accountIds
            });
        }

        public static void AddZoneField(Dictionary<string, GenericRuleCriteriaFieldValues> criteriaFields, List<object> zoneIds)
        {
            criteriaFields.Add("Zone", new StaticValues
            {

                Values = zoneIds
            });
        }

        public static void AddIsSameZoneField(Dictionary<string, GenericRuleCriteriaFieldValues> criteriaFields, long value)
        {
            criteriaFields.Add("IsSameZoneCall", new StaticValues
            {

                Values = new List<object> { value }
            });
        }
    }
}
