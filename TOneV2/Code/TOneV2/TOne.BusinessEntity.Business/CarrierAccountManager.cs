using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;
using Vanrise.Security.Business;

namespace TOne.BusinessEntity.Business
{
    public class CarrierAccountManager
    {
        ICarrierAccountDataManager _dataManager;
        public CarrierAccountManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
        }

        public List<CarrierInfo> GetCarriers(CarrierType carrierType, bool isAssignedCarrier)
        {
            if (isAssignedCarrier && carrierType == CarrierType.Customer)
            {
                  AccountManagerManager accountManagerManager = new AccountManagerManager();
                  List<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierType.Customer);
                  List<string> cutomers = new List<string>();
                  foreach (AssignedCarrier assignedCarrier in assignedCarriers)
                     {
                       cutomers.Add(assignedCarrier.CarrierAccountId);
                    }
                  return _dataManager.GetCarriers(carrierType, cutomers);
            }
            else if (isAssignedCarrier && carrierType == CarrierType.Supplier)
            {
                 AccountManagerManager accountManagerManager = new AccountManagerManager();
                 List<AssignedCarrier> assignedCarriers = accountManagerManager.GetAssignedCarriers(SecurityContext.Current.GetLoggedInUserId(), true, CarrierType.Supplier);
                 List<string> suppliers = new List<string>();
                 foreach (AssignedCarrier assignedCarrier in assignedCarriers)
                    {
                        suppliers.Add(assignedCarrier.CarrierAccountId);
                    }
                 return _dataManager.GetCarriers(carrierType, suppliers);
            }
            else
            return _dataManager.GetCarriers(carrierType,null);
        }

        public List<CarrierAccount> GetAllCarriers(CarrierType carrierType)
        {
            return _dataManager.GetAllCarriers(carrierType);
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

        public Vanrise.Entities.IDataRetrievalResult<CarrierAccount> GetFilteredCarrierAccounts(Vanrise.Entities.DataRetrievalInput<CarrierAccountQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _dataManager.GetFilteredCarrierAccounts(input));
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
        public int UpdateCarrierAccountGroup(CarrierAccount carrierAccount)
        {
            return _dataManager.UpdateCarrierAccountGroup(carrierAccount);
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
