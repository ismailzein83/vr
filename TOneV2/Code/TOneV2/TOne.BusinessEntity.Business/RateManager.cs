using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class RateManager
    {
        IRateDataManager _dataManager;

        public RateManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IRateDataManager>();
        }
        public void LoadCalculatedZoneRates(DateTime effectiveTime, bool isFuture, int batchSize, Action<ZoneRateBatch> onBatchAvailable)
        {
            _dataManager.LoadCalculatedZoneRates(effectiveTime, isFuture, batchSize, onBatchAvailable);
        }
        public void GetCalculatedZoneRates(DateTime effectiveTime, bool isFuture, IEnumerable<int> zoneIds, out List<ZoneRate> customerZoneRates, out List<ZoneRate> supplierZoneRates)
        {
            _dataManager.GetCalculatedZoneRates(effectiveTime, isFuture, zoneIds, out customerZoneRates, out supplierZoneRates);
        }
        public List<ExchangeRate> GetExchangeRates(DateTime Date)
        {
            return _dataManager.GetExchangeRates(Date);
        }

        public void UpdateRateEED(List<Rate> rates, string customerId)
        {
            _dataManager.UpdateRateEED(rates, customerId);
        }

        public void SaveRates(List<Rate> rates)
        {
            _dataManager.SaveRates(rates);
        }
    }
}
