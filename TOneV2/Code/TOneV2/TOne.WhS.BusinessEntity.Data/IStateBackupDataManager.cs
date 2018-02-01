using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface IStateBackupDataManager : IDataManager
    {
        object BackupData(StateBackupType backupType);

        bool RestoreData(long stateBackupId, StateBackupType stateBackupType, int userId);

        IEnumerable<StateBackup> GetFilteredStateBackups(StateBackupQuery input);

        StateBackup GetStateBackup(long stateBackupId);
        IEnumerable<StateBackup> GetStateBackupsAfterId(long stateBackupId);
    }
}
