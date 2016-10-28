using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class StateBackupBehavior
    {
        public abstract string GetDescription(IStateBackupContext context);

        public abstract bool IsMatch(IStateBackupContext context, object filter);
    }
}
