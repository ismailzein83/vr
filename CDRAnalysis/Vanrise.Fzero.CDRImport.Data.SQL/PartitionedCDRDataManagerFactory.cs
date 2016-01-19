using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.CDRImport.Data.SQL
{
    public static class PartitionedCDRDataManagerFactory
    {
        public static T GetCDRDataManager<T>(DateTime cdrTime, bool isReadonly) where T : PartitionedCDRDataManager
        {
            DateTime fromTime = PartitionedCDRDataManager.GetDBFromTime(cdrTime.Date);

            CDRDatabaseDataManager cdrDatabaseDataManager = new CDRDatabaseDataManager();
            
            CDRDatabaseInfo databaseInfo;
            if (!cdrDatabaseDataManager.TryGetReadyDatabase(fromTime, out databaseInfo))
            {
                if (isReadonly)
                    return null;
                cdrDatabaseDataManager.CreateNewDBIfNotCreated(fromTime);
                cdrDatabaseDataManager.TryGetReadyDatabase(fromTime, out databaseInfo);
            }
            
            var dataManager = Activator.CreateInstance<T>();
            dataManager.DatabaseName = databaseInfo.Settings.DatabaseName;
            return dataManager;
        }
    }
}
