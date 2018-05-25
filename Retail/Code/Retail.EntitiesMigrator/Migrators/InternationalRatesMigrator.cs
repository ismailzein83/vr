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


            //List<GenericRule> tariffRules = new List<GenericRule>();
            List<GenericRule> rateRules = new List<GenericRule>();

            //var defaultRateRule = Helper.CreateRateValueRule(Helper.OnNetRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { Rate = 0 });
            //var defaultTariffRule = Helper.CreateTariffRule(Helper.IntlRuleDefinition, GetDefaultCriteriaFieldValues(), new RateDetails { FractionUnit = 60 });
            //tariffRules.Add(defaultTariffRule);
            //rateRules.Add(defaultRateRule);
            HashSet<long> addedZones = new HashSet<long>();
            foreach (InternationalRate internationalRate in internationalRates)
            {

                SaleZone saleZone;
                if (_Zones.TryGetValue(internationalRate.ZoneName, out saleZone) && !addedZones.Contains(saleZone.SaleZoneId))
                {
                    addedZones.Add(saleZone.SaleZoneId);
                    rateRules.Add(GetRateValueRuleDetails(saleZone.SaleZoneId, internationalRate.InternationalRateDetail, internationalRate.BED));
                    //if (internationalRate.InternationalRateDetail.FractionUnit != 60)
                    //    tariffRules.Add(GetTariffRuleDetails(saleZone.SaleZoneId, internationalRate.InternationalRateDetail));
                }


            }
            //Helper.SaveTariffRules(tariffRules);
            Helper.SaveRateValueRules(rateRules);

        }

        private RateValueRule GetRateValueRuleDetails(long zoneId, RateDetails rateDetails, DateTime bed)
        {
            return Helper.CreateRateValueRule(Helper.IntlRuleDefinition, GetCriteriaFieldsValues(zoneId), rateDetails, bed, null);
        }
        private TariffRule GetTariffRuleDetails(long zoneId, RateDetails rateDetails)
        {
            return Helper.CreateTariffRule(Helper.IntlRuleDefinition, GetCriteriaFieldsValues(zoneId), rateDetails);
        }
        private Dictionary<string, GenericRuleCriteriaFieldValues> GetCriteriaFieldsValues(long zoneId)
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = GetDefaultCriteriaFieldValues();
            Helper.AddZoneField(result, new List<object> { zoneId });

            return result;
        }

        private Dictionary<string, GenericRuleCriteriaFieldValues> GetDefaultCriteriaFieldValues()
        {
            Dictionary<string, GenericRuleCriteriaFieldValues> result = Helper.BuildCriteriaFieldsValues(Helper.IntlRuleDefinition.ServiceTypeId, Helper.IntlRuleDefinition.ChargingPolicyId, MultiNet.Business.TrafficDirection.OutGoing);
            return result;
        }
    }
}
