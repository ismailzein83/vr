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
    }
}
