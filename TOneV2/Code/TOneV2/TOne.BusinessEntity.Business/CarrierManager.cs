using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;

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
        public CarrierAccount GetCarrierAccount(string carrierAccountId)
        {
            return _dataManager.GetCarrierAccount(carrierAccountId);
        }
        public int UpdateCarrierAccount(CarrierAccount carrierAccount)
        {
            return _dataManager.UpdateCarrierAccount(carrierAccount);
        }
    }
}
