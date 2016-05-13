using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;


namespace TOne.WhS.DBSync.Business
{
    public class CarrierAccountDBSyncManager
    {

        bool _UseTempTables;
        public CarrierAccountDBSyncManager(bool useTempTables)
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCarrierAccountsToTemp(List<CarrierAccount> carrierAccounts)
        {
            CarrierAccountDBSyncDataManager dataManager = new CarrierAccountDBSyncDataManager(_UseTempTables);
            dataManager.ApplyCarrierAccountsToTemp(carrierAccounts);
        }


        public List<CarrierAccount> GetCarrierAccounts()
        {
            CarrierAccountDBSyncDataManager dataManager = new CarrierAccountDBSyncDataManager(_UseTempTables);
            return dataManager.GetCarrierAccounts();
        }
    }
}
