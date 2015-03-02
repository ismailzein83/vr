using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class PricingGeneratorManager
    {
        public List<Commission> GetCommission(string customerId, int zoneId, DateTime when)
        {
            ICommissionDataManager dataManager = BEDataManagerFactory.GetDataManager<ICommissionDataManager>();
            return dataManager.GetCommission(customerId, zoneId, when);
        }
        public List<ToDConsideration> GetTOD(string customerId, int zoneId, DateTime when)
        {
            IToDConsiderationDataManager dataManager = BEDataManagerFactory.GetDataManager<IToDConsiderationDataManager>();
            return dataManager.GetToDConsideration(customerId, zoneId, when); ;
        }
        public List<Tariff> GetTariff(string customerId, int zoneId, DateTime when)
        {
            ITariffDataManager dataManager = BEDataManagerFactory.GetDataManager<ITariffDataManager>();
            return dataManager.GetTariff(customerId, zoneId, when);
        }
        public List<Rate> GetRates(string customerId, int zoneId, DateTime when, bool IsRepricing)
        {
            IRateDataManager dataManager = BEDataManagerFactory.GetDataManager<IRateDataManager>();
            return dataManager.GetRate(zoneId, customerId, when);
        }

        public void Dispose()
        {
        }
    }
}
