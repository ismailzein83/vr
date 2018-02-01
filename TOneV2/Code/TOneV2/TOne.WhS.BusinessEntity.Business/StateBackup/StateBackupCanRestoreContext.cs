using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    class StateBackupCanRestoreContext : IStateBackupCanRestoreContext
    {
        public long StateBackupId { get; set; }

        public string ErrorMessage { get; set; }

        public StateBackupType StateBackupType { get; set; }
    }

}
