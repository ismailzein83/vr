using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Data.SQL;


namespace TOne.WhS.DBSync.Business
{
    public class CarrierProfileDBSyncManager
    {

        bool _UseTempTables;
        public CarrierProfileDBSyncManager(bool useTempTables)
        {
            _UseTempTables = useTempTables;
        }

        public void ApplyCarrierProfilesToTemp(List<CarrierProfile> carrierProfiles)
        {
            CarrierProfileDBSyncDataManager dataManager = new CarrierProfileDBSyncDataManager(_UseTempTables);
            dataManager.ApplyCarrierProfilesToTemp(carrierProfiles);
        }


        public List<CarrierProfile> GetCarrierProfiles()
        {
            CarrierProfileDBSyncDataManager dataManager = new CarrierProfileDBSyncDataManager(_UseTempTables);
            return dataManager.GetCarrierProfiles();
        }
    }
}
