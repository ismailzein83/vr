using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;

namespace TOne.BusinessEntity.Business
{
    public class CarrierManager
    {
        ICarrierDataManager _dataManager;
        public CarrierManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICarrierDataManager>();
        }

        public List<CarrierInfo> GetCarriers(CarrierType carrierType)
        {
            return _dataManager.GetCarriers(carrierType);
        }

        public int InsertCarrierTest(string CarrierAccountID, string Name)
        {
            return _dataManager.InsertCarrierTest(CarrierAccountID, Name);
        }
        public List<CarrierAccount> GetCarrierAccounts(string name, string companyName, int from, int to)
        {
            return _dataManager.GetCarrierAccounts(name, companyName, from, to);
        }

        public Dictionary<string, CarrierAccount> GetAllCarrierAccounts()
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            return cacheManager.GetOrCreateObject("GetAllCarrierAccounts",
                TOne.Entities.CacheObjectType.CarrierAccount,
                () => _dataManager.GetAllCarrierAccounts()
                    );
        }

        public Dictionary<int, CarrierGroup> GetAllCarrierGroups()
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            return cacheManager.GetOrCreateObject("GetAllCarrierGroups",
                TOne.Entities.CacheObjectType.CarrierGroup,
                () => _dataManager.GetAllCarrierGroups()
                    );
        }

        public CarrierAccount GetCarrierAccount(string carrierAccountId)
        {
            CarrierAccount carrierAccount;
            GetAllCarrierAccounts().TryGetValue(carrierAccountId, out carrierAccount);
            return carrierAccount;
        }
        public int UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            return _dataManager.UpdateCarrierAccount(carrierAccount);
        }

        //public List<CarrierAccount> GetAllCarriers()
        //{
        //    return _dataManager.GetAllCarriers();
        //}

        //public static Dictionary<int, CarrierGroup> GetCarrierGroups(CarrierAccount CarrierAccount)
        //{

        //    char[] Saperator = { ',' };
        //    Dictionary<int, CarrierGroup> lstcarrierGroups = new Dictionary<int, CarrierGroup>();
        //    CarrierGroup carrierGroup;


        //    if (CarrierAccount.CarrierGroups == null || CarrierAccount.CarrierGroups == "") return null;
        //    if (CarrierAccount.CarrierGroups.ToString().Split(Saperator).Length == 0) return null;


        //    foreach (string CarrierGroupID in CarrierAccount.CarrierGroups.ToString().Split(Saperator))
        //    {
        //        carrierGroup = CarrierGroup.All[int.Parse(CarrierGroupID)];
        //        lstcarrierGroups[carrierGroup.CarrierGroupID] = carrierGroup;
        //    }


        //    return lstcarrierGroups;
        //}
    }
}
