using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class StateBackupBehavior
    {
        public virtual string GetDescription(IStateBackupContext context)
        {
            if (context.Data != null)
            {
                if (context.Data.OnRestoreStateBackupId.HasValue)
                    return String.Format("Backup for restore point with ID {0}",
                        context.Data.OnRestoreStateBackupId.Value);
            }
            return null;
        }

        public abstract bool IsMatch(IStateBackupContext context, object filter);
        public abstract bool CanRestore(IStateBackupCanRestoreContext context);
    }
}
