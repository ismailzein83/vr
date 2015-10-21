using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneManager
    {

        //public List<SupplierZoneInfo> GetSupplierZones(int supplierId, DateTime effectiveDate)
        //{
        //    ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
        //    return dataManager.GetSupplierZones(supplierId, effectiveDate);
        //}

        public List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.GetSupplierZones(supplierId, effectiveDate);
        }
        public SupplierZone GetSupplierZone(long zoneId)
        {
            throw new NotImplementedException();
        }


        public long ReserveIDRange(int numberOfIDs)
        {
            ISupplierZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneDataManager>();
            return dataManager.ReserveIDRange(numberOfIDs);
        }
        #region Private Members

        List<CarrierAccountDetail> GetCachedCarrierAccounts()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetCarrierAccounts",
               () =>
               {
                   ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
                   return dataManager.GetCarrierAccounts();
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ICarrierAccountDataManager _dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreCarrierAccountsUpdated(ref _updateHandle);
            }
        }

        private CarrierAccountInfo CarrierAccountInfoMapper(CarrierAccountDetail carrierAccountDetail)
        {
            return new CarrierAccountInfo()
            {
                CarrierAccountId = carrierAccountDetail.CarrierAccountId,
                Name = carrierAccountDetail.Name,
            };
        }

        #endregion

    }
}
