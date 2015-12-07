using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Sales.Entities;

namespace TOne.Sales.Data
{
    public interface IStateBackupDataManager : IDataManager
    {
        StateBackup Create(StateBackupType backupType, string carrierAccountId);

        bool Save(StateBackup stateBackup, out int stateBackupId);
    }
}
