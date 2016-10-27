using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierStateBackupBehavior : StateBackupBehavior
    {
        public override string GetDescription(IStateBackupContext context)
        {
            if (context.Data == null)
                return string.Empty;

            StateBackupSupplier backupData = context.Data as StateBackupSupplier;
            return string.Empty;
        }
    }
}
