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
    }
}
