using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;

namespace TOne.BusinessEntity.Business
{
    public class ZoneManager
    { 
        IZoneDataManager _dataManager;

        public ZoneManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
        }
        public void LoadZonesInfo(DateTime effectiveTime, bool isFuture, List<Entities.CarrierAccountInfo> activeSuppliers, int batchSize, Action<List<Entities.ZoneInfo>> onBatchAvailable)
        {
            _dataManager.LoadZonesInfo(effectiveTime, isFuture, activeSuppliers, batchSize, onBatchAvailable);
        }
    }
}
