using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

namespace TOne.BusinessEntity.Business
{
    public class SaleZoneMarketPriceManager
    {
        ISaleZoneMarketPriceDataManager _dataManager;
        public SaleZoneMarketPriceManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneMarketPriceDataManager>();
        }
        public IEnumerable<SaleZoneMarketPrice> GetSaleZoneMarketPrices()
        {
            return _dataManager.GetSaleZoneMarketPrices();
        }
        public SaleZoneMarketPrices GetAllSaleZoneMarketPrices()
        {
            return _dataManager.GetAllSaleZoneMarketPrices();
        }
    }
}
