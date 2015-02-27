using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TABS;
using TOne.BusinessEntity.Data;

namespace TOne.BusinessEntity.Business
{
    public class PricingGeneratorEntites<T> : IDisposable
    {
        public static List<T> Load(string customerId, int zoneId, DateTime when, bool IsRepricing)
        {
            if (typeof(T) == typeof(Rate))
                return GetRates(customerId, zoneId, when, IsRepricing) as List<T>;
            if (typeof(T) == typeof(Tariff))
                return GetTarrif(customerId, zoneId, when) as List<T>;
            if (typeof(T) == typeof(ToDConsideration))
                return GetTOD(customerId, zoneId, when) as List<T>;
            if (typeof(T) == typeof(Commission))
                return GetCommission(customerId, zoneId, when) as List<T>;

            return new List<T>();
        }
        private static List<Commission> GetCommission(string customerId, int zoneId, DateTime when)
        {
            List<Commission> Commission = new List<Commission>();


            return Commission;
            //ICommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ICommissionDataManager>();
            //return dataManager.GetCommission(customerId, zoneId, when);
        }
        private static List<ToDConsideration> GetTOD(string customerId, int zoneId, DateTime when)
        {
            List<ToDConsideration> ToDConsideration = new List<ToDConsideration>();


            return ToDConsideration;
        }
        private static List<Tariff> GetTarrif(string customerId, int zoneId, DateTime when)
        {
            List<Tariff> Tarrif = new List<Tariff>();
            
            return Tarrif;
        }
        private static List<Rate> GetRates(string customerId, int zoneId, DateTime when, bool IsRepricing)
        {
            List<Rate> Rates = new List<Rate>();
            
            //IRateDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateDataManager>();
            //return dataManager.GetRate(zoneId, customerId, when);
            return Rates;
        }

        public void Dispose()
        {
        }
    }
}
