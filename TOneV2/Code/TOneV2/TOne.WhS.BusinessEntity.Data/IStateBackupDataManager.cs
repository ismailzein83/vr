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
        void BackupData(StateBackupType backupType);

        void RestoreData(int stateBackupId);
    }
}
