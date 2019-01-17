using System;

namespace TOne.WhS.BusinessEntity.Data.RDB
{
    class StateBackupSupplierBehavior : StateBackupTypeBehavior
    {
        public override string GetBackupCommands(long stateBackupId)
        {
            throw new NotImplementedException();
        }

        public override string GetRestoreCommands(long stateBackupId)
        {
            throw new NotImplementedException();
        }
    }
}
