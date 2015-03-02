using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Entities;

namespace TOne.CDR.Business
{
    public class PricingGeneratorEntities<T> : IDisposable
    {
        public static List<T> Load(string customerId, int zoneId, DateTime when, bool IsRepricing)
        {
            TOne.BusinessEntity.Business.PricingGeneratorManager manager = new BusinessEntity.Business.PricingGeneratorManager();

            if (typeof(T) == typeof(Rate))
                return manager.GetRates(customerId, zoneId, when, IsRepricing) as List<T>;
            if (typeof(T) == typeof(Tariff))
                return manager.GetTariff(customerId, zoneId, when) as List<T>;
            if (typeof(T) == typeof(ToDConsideration))
                return manager.GetTOD(customerId, zoneId, when) as List<T>;
            if (typeof(T) == typeof(Commission))
                return manager.GetCommission(customerId, zoneId, when) as List<T>;

            return new List<T>();
        }

        public void Dispose()
        {
        }
    }
}
