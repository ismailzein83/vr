using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class StateBackupManager
    {
        public void BackupData(StateBackupType backupType)
        {
            IStateBackupDataManager manager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
            manager.BackupData(backupType);
        }

        public void RestoreData(StateBackupType backupType, int stateBackupId)
        {
            IStateBackupDataManager manager = BEDataManagerFactory.GetDataManager<IStateBackupDataManager>();
            manager.RestoreData(stateBackupId);
        }
   
    }
}
