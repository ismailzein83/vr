using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class StateBackupType
    {
        public abstract Guid ConfigId { get; }

        public int UserId { get; set; }
    }
}
