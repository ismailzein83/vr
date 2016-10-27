using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class StateBackupSupplier : StateBackupType
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public int SupplierId { get; set; }
    }
}
