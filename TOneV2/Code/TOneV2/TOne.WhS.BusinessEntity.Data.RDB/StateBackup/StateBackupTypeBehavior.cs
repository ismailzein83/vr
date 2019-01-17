using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    public abstract class StateBackupTypeBehavior
    {
        public string BackupDatabaseName { get; set; }

        public StateBackupType Data { get; set; }

        public abstract string GetBackupCommands(long stateBackupId);

        public abstract string GetRestoreCommands(long stateBackupId);
    }
}
