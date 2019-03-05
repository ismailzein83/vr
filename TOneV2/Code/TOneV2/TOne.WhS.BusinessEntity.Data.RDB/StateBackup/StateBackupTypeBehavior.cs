using Vanrise.Data.RDB;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public abstract class StateBackupTypeBehavior
    {
        public string BackupDatabaseName { get; set; }

        public StateBackupType Data { get; set; }

        public abstract void GetBackupQueryContext(RDBQueryContext queryContext, long stateBackupId);

        public abstract void GetRestoreCommands(RDBQueryContext queryContext, long stateBackupId);
    }
}
