using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.SQL
{
    public abstract class StateBackupTypeBehavior
    {
        public string BackupDatabaseName { get; set; }

        public StateBackupType Data { get; set; }

        public abstract string GetBackupCommands(int stateBackupId);

        public abstract string GetRestoreCommands(int stateBackupId);
    }
}
